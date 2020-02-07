using System;
using System.Collections;
using System.Collections.Generic;
using BeauRoutine;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Runtime
{
    internal class ExecutionScope : IScriptContext, IRefCounted
    {
        [Flags]
        internal enum Type : byte
        {
            Registers = 0x01,
            Pooled = 0x02
        }

        internal readonly RSEnvironment m_Environment;
        internal readonly Type m_Type;
        private readonly RSValue[] m_Registers;

        private IRSRuntimeEntity m_Self;
        private RSValue m_Argument;

        internal ExecutionScope(RSEnvironment inEnvironment, Type inType)
        {
            m_Environment = inEnvironment;
            m_Type = inType;

            if ((m_Type & Type.Registers) != 0)
            {
                m_Registers = new RSValue[RSRegisters.MaxRegisters];
            }
        }

        public void Initialize(IRSRuntimeEntity inSelf, RSValue inArgument)
        {
            m_Self = inSelf;
            m_Argument = inArgument;
        }

        public void Reset()
        {
            m_Self = null;
            m_Argument = RSValue.Null;

            if (m_Registers != null)
            {
                for (int i = 0; i < RSRegisters.MaxRegisters; ++i)
                {
                    m_Registers[i] = default(RSValue);
                }
            }
        }

        public IRSRuntimeEntity SelfEntity { get { return m_Self; } }
        public RSValue Argument { get { return m_Argument; } }

        #region Resolution

        /// <summary>
        /// Resolves scope to an entity or set of entities.
        /// </summary>
        public MultiReturn<IRSRuntimeEntity> ResolveEntity(EntityScopeData inScope)
        {
            MultiReturn<IRSRuntimeEntity> returnVal;
            switch (inScope.Type)
            {
                case EntityScopeType.Null:
                    {
                        returnVal = MultiReturn<IRSRuntimeEntity>.Default;
                        break;
                    }
                case EntityScopeType.Self:
                    {
                        returnVal = new MultiReturn<IRSRuntimeEntity>(m_Self);
                        break;
                    }
                case EntityScopeType.Global:
                    {
                        return new MultiReturn<IRSRuntimeEntity>();
                    }
                case EntityScopeType.Argument:
                    {
                        returnVal = ResolveEntity(m_Argument.AsEntity);
                        break;
                    }

                case EntityScopeType.ObjectById:
                    {
                        returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntityWithId(inScope.IdArg));
                        break;
                    }
                case EntityScopeType.ObjectInRegister:
                    {
                        EntityScopeData id = PeekRegister(inScope.RegisterArg).AsEntity;
                        returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntityWithId(id.IdArg));
                        break;
                    }

                case EntityScopeType.ObjectsWithGroup:
                    {
                        if (inScope.UseFirst)
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntityWithGroup(inScope.GroupArg));
                        }
                        else
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntitiesWithGroup(inScope.GroupArg));
                        }
                        break;
                    }

                case EntityScopeType.ObjectsWithName:
                    {
                        if (inScope.UseFirst)
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntityWithName(inScope.SearchArg));
                        }
                        else
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntitiesWithName(inScope.SearchArg));
                        }
                        break;
                    }

                case EntityScopeType.ObjectsWithPrefab:
                    {
                        if (inScope.UseFirst)
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntityWithPrefab(inScope.SearchArg));
                        }
                        else
                        {
                            returnVal = new MultiReturn<IRSRuntimeEntity>(m_Environment.Entities.Lookup.EntitiesWithPrefab(inScope.SearchArg));
                        }
                        break;
                    }

                case EntityScopeType.Invalid:
                    {
                        Assert.Fail("Missing entity reference");
                        return MultiReturn<IRSRuntimeEntity>.Default;
                    }

                default:
                    {
                        Assert.Fail("Unrecognized scope type {0}", inScope.Type);
                        return MultiReturn<IRSRuntimeEntity>.Default;
                    }
            }

            return ResolveEntityLinks(returnVal, inScope.LinksArg, inScope.UseFirstLink);
        }

        /// <summary>
        /// Resolves argument to a value.
        /// </summary>
        public MultiReturn<RSValue> ResolveValue(RSResolvableValueData inArgument)
        {
            switch (inArgument.Mode)
            {
                case ResolvableValueMode.Value:
                    return inArgument.Value;
                case ResolvableValueMode.Argument:
                    return m_Argument;
                case ResolvableValueMode.Register:
                    return PeekRegister(inArgument.Register);
                case ResolvableValueMode.Query:
                    return EvaluateQuery(inArgument.Query, inArgument.QueryArguments);

                default:
                    Assert.Fail("Unrecognized argument mode {0}", inArgument.Mode);
                    return RSValue.Null;
            }
        }

        /// <summary>
        /// Resolves argument to a value.
        /// </summary>
        public MultiReturn<RSValue> ResolveValue(NestedValue inArgument)
        {
            switch (inArgument.Mode)
            {
                case ResolvableValueMode.Value:
                    return inArgument.Value;
                case ResolvableValueMode.Argument:
                    return m_Argument;
                case ResolvableValueMode.Register:
                    return PeekRegister(inArgument.Register);
                case ResolvableValueMode.Query:
                    return EvaluateQuery(inArgument.Query, null);

                default:
                    Assert.Fail("Unrecognized argument mode {0}", inArgument.Mode);
                    return RSValue.Null;
            }
        }

        private void ResolveArgsArray(RSResolvableValueData[] inArguments, RSValue[] ioResults)
        {
            if (inArguments == null)
                return;

            Assert.True(inArguments.Length == ioResults.Length, "Mismatched input->output arg conversion length");

            for (int i = 0; i < ioResults.Length; ++i)
                ioResults[i] = ResolveValue(inArguments[i]).ForceSingle();
        }

        private void ResolveArgsArray(NestedValue[] inArguments, RSValue[] ioResults)
        {
            if (inArguments == null)
                return;

            Assert.True(inArguments.Length == ioResults.Length, "Mismatched input->output arg conversion length");

            for (int i = 0; i < ioResults.Length; ++i)
                ioResults[i] = ResolveValue(inArguments[i]).ForceSingle();
        }

        private MultiReturn<IRSRuntimeEntity> ResolveEntityLinks(MultiReturn<IRSRuntimeEntity> inEntities, string inLinks, bool inbUseFirstLink)
        {
            if (string.IsNullOrEmpty(inLinks))
                return inEntities;

            return new MultiReturn<IRSRuntimeEntity>(EnumerateEntityLinks(inEntities, inLinks, inbUseFirstLink));
        }

        private IEnumerable<IRSRuntimeEntity> EnumerateEntityLinks(MultiReturn<IRSRuntimeEntity> inEntities, string inLinks, bool inbUseFirstLink)
        {
            using(PooledList<StringSlice> paths = PooledList<StringSlice>.Alloc())
            {
                int sliceCount = StringSlice.Split(inLinks, EntityLinkSplitChars, StringSplitOptions.None, paths);

                foreach (var entity in inEntities)
                {
                    IRSRuntimeEntity currentEntity = entity;
                    for (int i = 0; currentEntity != null && i < sliceCount - 1; ++i)
                    {
                        StringSlice path = paths[i];
                        currentEntity = currentEntity.Links?.EntityWithLink(path);
                    }

                    if (currentEntity != null)
                    {
                        var links = currentEntity.Links;
                        if (links == null)
                            continue;

                        if (inbUseFirstLink)
                        {
                            yield return links.EntityWithLink(paths[sliceCount - 1]);
                        }
                        else
                        {
                            foreach (var linkedEntity in links.EntitiesWithLink(paths[sliceCount - 1]))
                            {
                                yield return linkedEntity;
                            }
                        }
                    }
                }
            }
        }

        static private readonly char[] EntityLinkSplitChars = new char[] { '.' };

        #endregion // Resolution

        #region Registers

        public void LoadRegister(RegisterIndex inRegister, RSValue inValue)
        {
            Assert.True(m_Registers != null, "Execution Scope has no registers available");
            m_Registers[(int) inRegister] = inValue;
        }

        public RSValue PeekRegister(RegisterIndex inRegister)
        {
            Assert.True(m_Registers != null, "Execution Scope has no registers available");
            return m_Registers[(int) inRegister];
        }

        public void ClearRegisters()
        {
            Assert.True(m_Registers != null, "Execution Scope has no registers available");
            for (int i = 0; i < RSRegisters.MaxRegisters; ++i)
            {
                m_Registers[i] = default(RSValue);
            }
        }

        #endregion // Registers

        #region Queries

        /// <summary>
        /// Resolves a query to a set of values.
        /// </summary>
        public MultiReturn<RSValue> EvaluateQuery(EntityScopedIdentifier inQuery, NestedValue[] inArguments)
        {
            RSQueryInfo queryInfo = m_Environment.Library.GetQuery(inQuery.Id);
            MultiReturn<IRSRuntimeEntity> targets = ResolveEntity(inQuery.Scope);
            ResolveArgsArray(inArguments, queryInfo.TempArgStorage);

            if (targets.Set != null)
                return new MultiReturn<RSValue>(m_Environment.EvaluateQueries(targets.Set, queryInfo, queryInfo.TempArgStorage, this));

            return new MultiReturn<RSValue>(m_Environment.EvaluateQuery(targets.Single, queryInfo, queryInfo.TempArgStorage, this));
        }

        #endregion // Queries

        #region Conditions

        /// <summary>
        /// Evaluates a set of conditions.
        /// </summary>
        public bool EvaluateConditions(RSConditionData[] inConditions, Subset inSubset)
        {
            if (inConditions == null)
                return true;

            int activeConditions = 0;
            for (int i = 0, numConditions = inConditions.Length; i < numConditions; ++i)
            {
                RSConditionData condition = inConditions[i];
                if (!condition.Enabled)
                    continue;

                ++activeConditions;
                bool bPass = EvaluateCondition(condition);
                switch (inSubset)
                {
                    case Subset.All:
                        {
                            if (!bPass)
                                return false;
                            break;
                        }

                    case Subset.Any:
                        {
                            if (bPass)
                                return true;
                            break;
                        }

                    case Subset.None:
                        {
                            if (bPass)
                                return false;
                            break;
                        }
                }
            }

            if (activeConditions == 0)
                return true;

            switch (inSubset)
            {
                case Subset.All:
                case Subset.None:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Evaluates a condition.
        /// </summary>
        public bool EvaluateCondition(RSConditionData inCondition)
        {
            Assert.True(!inCondition.Target.IsMultiValue(), "Multi-target queries cannot be right-hand values for conditions");

            MultiReturn<RSValue> query = ResolveValue(inCondition.Query);
            RSValue target = ResolveValue(inCondition.Target).ForceSingle();
            CompareOperator op = inCondition.Operator;

            if (query.Set != null)
            {
                Subset subset = inCondition.MultiQuerySubset;
                foreach (var check in query.Set)
                {
                    bool bPass = Evaluate(op, check, target);
                    switch (subset)
                    {
                        case Subset.All:
                            {
                                if (!bPass)
                                    return false;
                                break;
                            }

                        case Subset.Any:
                            {
                                if (bPass)
                                    return true;
                                break;
                            }

                        case Subset.None:
                            {
                                if (bPass)
                                    return false;
                                break;
                            }
                    }
                }

                switch (subset)
                {
                    case Subset.All:
                    case Subset.None:
                        return true;
                    default:
                        return false;
                }
            }

            return Evaluate(op, query.Single, target);
        }

        public bool Evaluate(CompareOperator inComparison, RSValue inCheck, RSValue inValue)
        {
            switch (inCheck.GetInnerType())
            {
                case RSValue.InnerType.Null:
                    return ComparisonExtensions.Evaluate(inComparison, false, inCheck.GetInnerType() != RSValue.InnerType.Null);
                case RSValue.InnerType.Int32:
                    return ComparisonExtensions.Evaluate(inComparison, inCheck.AsInt, inValue.AsInt);
                case RSValue.InnerType.Bool:
                    return ComparisonExtensions.Evaluate(inComparison, inCheck.AsBool, inValue.AsBool);
                case RSValue.InnerType.Color:
                    return ComparisonExtensions.EvaluateEquatable(inComparison, inCheck.AsColor, inValue.AsColor);
                case RSValue.InnerType.Float:
                    return ComparisonExtensions.Evaluate(inComparison, inCheck.AsFloat, inValue.AsFloat);
                case RSValue.InnerType.Vector2:
                    return ComparisonExtensions.EvaluateEquatable(inComparison, inCheck.AsVector2, inValue.AsVector2);
                case RSValue.InnerType.Vector3:
                    return ComparisonExtensions.EvaluateEquatable(inComparison, inCheck.AsVector3, inValue.AsVector3);
                case RSValue.InnerType.Vector4:
                    return ComparisonExtensions.EvaluateEquatable(inComparison, inCheck.AsVector4, inValue.AsVector4);
                case RSValue.InnerType.String:
                    return ComparisonExtensions.Evaluate(inComparison, inCheck.AsString, inValue.AsString);
                case RSValue.InnerType.Enum:
                    return ComparisonExtensions.Evaluate(inComparison, inCheck.AsInt, inValue.AsInt);
                case RSValue.InnerType.EntityScope:
                    return ComparisonExtensions.EvaluateReferenceEquals(inComparison, ResolveEntity(inCheck.AsEntity).ForceSingle(), ResolveEntity(inValue.AsEntity).ForceSingle());
                default:
                    throw new ArgumentException("Invalid value type " + inCheck.GetInnerType(), "inCheck");
            }
        }

        #endregion // Conditions

        #region Actions

        /// <summary>
        /// Performs an ordered sequence of actions.
        /// </summary>
        public IEnumerator PerformActions(RSActionData[] inActions)
        {
            using(new SharedRef<ExecutionScope>(this))
            {
                if (inActions == null || inActions.Length <= 0)
                    yield break;

                for (int i = 0; i < inActions.Length; ++i)
                {
                    if (!inActions[i].Enabled)
                        continue;

                    var ret = PerformAction(inActions[i]);

                    if (ret.Set != null)
                    {
                        PooledList<IEnumerator> waits = null;

                        foreach (var val in ret.Set)
                        {
                            switch (val.Type)
                            {
                                case ActionResultType.Iterator:
                                    {
                                        if (waits == null)
                                        {
                                            waits = PooledList<IEnumerator>.Alloc();
                                        }

                                        waits.Add((IEnumerator) val.Value);
                                        break;
                                    }
                            }
                        }

                        if (waits != null && waits.Count > 0)
                        {
                            IEnumerator combinedWait = Routine.Combine(waits);
                            waits.Dispose();
                            waits = null;

                            yield return combinedWait;
                        }
                    }
                    else
                    {
                        switch (ret.Single.Type)
                        {
                            case ActionResultType.Iterator:
                                yield return ret.Single.Value;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Performs an action on entities.
        /// </summary>
        public MultiReturn<ActionResult> PerformAction(RSActionData inAction)
        {
            RSActionInfo actionInfo = m_Environment.Library.GetAction(inAction.Action.Id);
            MultiReturn<IRSRuntimeEntity> targets = ResolveEntity(inAction.Action.Scope);
            ResolveArgsArray(inAction.Arguments, actionInfo.TempArgStorage);

            if (targets.Set != null)
                return new MultiReturn<ActionResult>(m_Environment.PerformActions(targets.Set, actionInfo, actionInfo.TempArgStorage, this));

            return new MultiReturn<ActionResult>(m_Environment.PerformAction(targets.Single, actionInfo, actionInfo.TempArgStorage, this));
        }

        #endregion // Actions

        #region IScriptContext

        public IRSDebugLogger Logger { get { return m_Environment.Logger; } }
        RSLibrary IScriptContext.Library { get { return m_Environment.Library; } }
        IRSEntityMgr IScriptContext.Entities { get { return m_Environment.Entities; } }
        IRSRuleTableMgr IScriptContext.Tables { get { return m_Environment.Tables; } }

        void IScriptContext.Trigger(IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, object inArgument, bool inbForce)
        {
            m_Environment.Trigger(inEntity, inTriggerId, RSInterop.ToRSValue(inArgument), inbForce);
        }

        void IScriptContext.Broadcast(RSTriggerId inTriggerId, object inArgument, bool inbForce)
        {
            m_Environment.Broadcast(inTriggerId, RSInterop.ToRSValue(inArgument), inbForce);
        }

        void IScriptContext.LoadRegister(RegisterIndex inRegister, object inValue)
        {
            LoadRegister(inRegister, RSInterop.ToRSValue(inValue));
        }

        object IScriptContext.PeekRegister(RegisterIndex inRegister)
        {
            return RSInterop.ToObject(PeekRegister(inRegister), this);
        }

        #endregion // IScriptContext

        #region IRefCounted

        int IRefCounted.ReferenceCount { get; set; }

        void IRefCounted.OnReferenced() { }

        void IRefCounted.OnReleased()
        {
            Reset();

            if ((m_Type & Type.Pooled) != 0)
            {
                m_Environment.RecycleScope(this);
            }
        }

        #endregion // IRefCounted
    }
}