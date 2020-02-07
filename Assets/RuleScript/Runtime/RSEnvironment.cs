using System.Collections.Generic;
using BeauRoutine;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Runtime
{
    public sealed class RSEnvironment : IScriptContext
    {
        #region Objects

        public readonly RSLibrary Library;
        public readonly IRSRuntimeEntityMgr Entities;
        public readonly IRSRuleTableMgr Tables;
        public readonly IRSDebugLogger Logger;

        #endregion // Objects

        private readonly ExecutionScope m_StaticScope;
        private bool m_Destroyed = false;

        private readonly List<ExecutionScope> m_LocalScopePool;
        private readonly List<ExecutionScope> m_RegisterScopePool;

        public RSEnvironment(RSLibrary inDatabase, IRSRuntimeEntityMgr inEntityMgr, IRSRuleTableMgr inTableMgr, IRSDebugLogger inLogger)
        {
            Library = inDatabase;
            Entities = inEntityMgr;
            Tables = inTableMgr;
            Logger = inLogger;

            if (Entities != null)
                Entities.Context = this;

            m_StaticScope = new ExecutionScope(this, ExecutionScope.Type.Registers);
            m_StaticScope.Initialize(null, RSValue.Null);

            m_LocalScopePool = new List<ExecutionScope>();
            m_RegisterScopePool = new List<ExecutionScope>();
        }

        public void PrewarmPools(int inScopeCount, int inRegisterScopeCount)
        {
            if (m_LocalScopePool.Capacity < inScopeCount)
            {
                m_LocalScopePool.Capacity = inScopeCount;
            }

            while (m_LocalScopePool.Count < inScopeCount)
            {
                ExecutionScope scope = new ExecutionScope(this, ExecutionScope.Type.Pooled);
                m_LocalScopePool.Add(scope);
            }

            if (m_RegisterScopePool.Capacity < inRegisterScopeCount)
            {
                m_RegisterScopePool.Capacity = inRegisterScopeCount;
            }

            while (m_RegisterScopePool.Count < inRegisterScopeCount)
            {
                ExecutionScope scope = new ExecutionScope(this, ExecutionScope.Type.Pooled | ExecutionScope.Type.Registers);
                m_RegisterScopePool.Add(scope);
            }
        }

        public void Destroy()
        {
            if (!m_Destroyed)
            {
                m_Destroyed = true;
                Entities?.Destroy();
            }
        }

        #region Queries

        /// <summary>
        /// Evaluates a query with static scope.
        /// </summary>
        public RSValue EvaluateQuery(string inQueryId, params object[] inArgs)
        {
            RSQueryInfo queryInfo = Library.GetQuery(inQueryId);
            ExecutionScope scope = m_StaticScope;
            CloneScopeIfNecessary(scope, TableUtils.GetRuleFlags(queryInfo.Flags), out scope);

            using(new SharedRef<ExecutionScope>(scope))
            {
                return EvaluateQuery(null, queryInfo, InternalScriptUtils.Convert(inArgs), scope);
            }
        }

        /// <summary>
        /// Evaluates a query with static scope.
        /// </summary>
        public RSValue EvaluateQuery(IRSRuntimeEntity inEntity, string inQueryId, params object[] inArgs)
        {
            RSQueryInfo queryInfo = Library.GetQuery(inQueryId);
            ExecutionScope scope = CreateScope(inEntity, RSValue.Null, TableUtils.GetRuleFlags(queryInfo.Flags));

            using(new SharedRef<ExecutionScope>(scope))
            {
                return EvaluateQuery(inEntity, queryInfo, InternalScriptUtils.Convert(inArgs), scope);
            }
        }

        internal RSValue EvaluateQuery(IRSRuntimeEntity inEntity, RSQueryInfo inQuery, RSValue[] inArgs, ExecutionScope inContext)
        {
            return inQuery.Invoke(inEntity, inArgs, inContext);
        }

        internal IEnumerable<RSValue> EvaluateQueries(IEnumerable<IRSRuntimeEntity> inEntities, RSQueryInfo inQuery, RSValue[] inArguments, ExecutionScope inContext)
        {
            inQuery.PrepArguments(inArguments, inContext);
            foreach (var entity in inEntities)
            {
                yield return inQuery.InvokeWithCachedArgs(entity, inContext);
            }
        }

        #endregion // Queries

        #region Actions

        /// <summary>
        /// Performs an action with static scope.
        /// </summary>
        public object PerformAction(string inActionId, params object[] inArgs)
        {
            RSActionInfo actionInfo = Library.GetAction(inActionId);
            ExecutionScope scope = m_StaticScope;
            CloneScopeIfNecessary(scope, TableUtils.GetRuleFlags(actionInfo.Flags), out scope);

            using(new SharedRef<ExecutionScope>(scope))
            {
                return PerformAction(null, actionInfo, InternalScriptUtils.Convert(inArgs), scope).Value;
            }
        }

        /// <summary>
        /// Performs an action with static scope.
        /// </summary>
        public object PerformAction(IRSRuntimeEntity inEntity, string inActionId, params object[] inArgs)
        {
            RSActionInfo actionInfo = Library.GetAction(inActionId);
            ExecutionScope scope = CreateScope(inEntity, RSValue.Null, TableUtils.GetRuleFlags(actionInfo.Flags));

            using(new SharedRef<ExecutionScope>(scope))
            {
                return PerformAction(inEntity, actionInfo, InternalScriptUtils.Convert(inArgs), scope).Value;
            }
        }

        internal ActionResult PerformAction(IRSRuntimeEntity inEntity, RSActionInfo inAction, RSValue[] inArguments, ExecutionScope inContext)
        {
            return inAction.Invoke(inEntity, inArguments, inContext);
        }

        internal IEnumerable<ActionResult> PerformActions(IEnumerable<IRSRuntimeEntity> inEntities, RSActionInfo inAction, RSValue[] inArguments, ExecutionScope inContext)
        {
            inAction.PrepArguments(inArguments, inContext);
            foreach (var entity in inEntities)
            {
                yield return inAction.InvokeWithCachedArgs(entity, inContext);
            }
        }

        #endregion // Actions

        #region Triggers

        public void Trigger(IRSRuntimeEntity inEntity, string inTriggerId, bool inbForce = false)
        {
            Trigger(inEntity, new RSTriggerId(ScriptUtils.Hash(inTriggerId)), RSValue.Null, inbForce);
        }

        internal void TriggerRS(IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, RSValue inArgument, bool inbForce)
        {
            if (!inbForce && inEntity.IsLocked())
                return;

            ExecutionScope scope = CreateScope(inEntity, inArgument, 0);
            using(new SharedRef<ExecutionScope>(scope))
            {
                inEntity?.RuleTable?.EvaluateTrigger(inTriggerId, scope);
            }
        }

        internal void Broadcast(RSTriggerId inTriggerId, RSValue inArgument, bool inbForce)
        {
            foreach (var entity in Entities.EntitiesForTrigger(inTriggerId))
            {
                TriggerRS(entity, inTriggerId, inArgument, inbForce);
            }
        }

        #endregion // Triggers

        #region Execution Scope

        internal ExecutionScope CreateScope(IRSRuntimeEntity inEntity, RSValue inArgument, RuleFlags inFlags)
        {
            List<ExecutionScope> pool;
            if ((inFlags & RuleFlags.UsesRegisters) != 0)
            {
                pool = m_RegisterScopePool;
            }
            else
            {
                pool = m_LocalScopePool;
            }

            ExecutionScope scope;
            int count = pool.Count;
            if (count > 0)
            {
                scope = pool[count - 1];
                pool.RemoveAt(count - 1);
            }
            else
            {
                scope = ConstructScope(inFlags);
            }

            scope.Initialize(inEntity, inArgument);
            return scope;
        }

        internal bool CloneScopeIfNecessary(ExecutionScope inScope, RuleFlags inFlags, out ExecutionScope outScope)
        {
            if ((inFlags & RuleFlags.UsesRegisters) != 0)
            {
                outScope = CloneScope(inScope, inFlags);
                return true;
            }

            outScope = inScope;
            return false;
        }

        internal ExecutionScope CloneScope(ExecutionScope inScope, RuleFlags inFlags)
        {
            Assert.True(inScope != null && inScope.m_Environment == this);
            return CreateScope(inScope.SelfEntity, inScope.Argument, inFlags);
        }

        internal void RecycleScope(ExecutionScope inScope)
        {
            Assert.True(inScope != null && inScope.m_Environment == this);
            if ((inScope.m_Type & ExecutionScope.Type.Registers) != 0)
            {
                m_RegisterScopePool.Add(inScope);
            }
            else
            {
                m_LocalScopePool.Add(inScope);
            }
        }

        private ExecutionScope ConstructScope(RuleFlags inFlags)
        {
            Logger?.Warn("Constructing new execution scope");
            if ((inFlags & RuleFlags.UsesRegisters) != 0)
            {
                return new ExecutionScope(this, ExecutionScope.Type.Pooled | ExecutionScope.Type.Registers);
            }

            return new ExecutionScope(this, ExecutionScope.Type.Pooled);
        }

        internal ExecutionScope StaticScope
        {
            get { return m_StaticScope; }
        }

        #endregion // Execution Scope

        #region IScriptContext

        IRSDebugLogger IScriptContext.Logger { get { return Logger; } }
        RSLibrary IScriptContext.Library { get { return Library; } }
        IRSEntityMgr IScriptContext.Entities { get { return Entities; } }
        IRSRuleTableMgr IScriptContext.Tables { get { return Tables; } }

        public void Trigger(IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, object inArgument, bool inbForce)
        {
            if (m_Destroyed)
                return;

            TriggerRS(inEntity, inTriggerId, RSInterop.ToRSValue(inArgument), inbForce);
        }

        public void Broadcast(RSTriggerId inTriggerId, object inArgument, bool inbForce)
        {
            if (m_Destroyed)
                return;

            Broadcast(inTriggerId, RSInterop.ToRSValue(inArgument), inbForce);
        }

        void IScriptContext.LoadRegister(RegisterIndex inRegister, object inValue)
        {
            throw new System.NotImplementedException("Registers are not valid on the RSEnvironment");
        }

        object IScriptContext.PeekRegister(RegisterIndex inRegister)
        {
            throw new System.NotImplementedException("Registers are not valid on the RSEnvironment");
        }

        #endregion // IScriptContext
    }
}