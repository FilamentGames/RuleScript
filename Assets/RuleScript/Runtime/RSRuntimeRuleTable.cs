using System;
using System.Collections.Generic;
using BeauData;
using BeauRoutine;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Runtime
{
    public sealed class RSRuntimeRuleTable
    {
        public delegate void TriggerDelegate(RSTriggerId inTriggerId, object inArgument);

        #region Types

        [Flags]
        internal enum RuleState : byte
        {
            Disabled = 0x001
        }

        #endregion // Types

        private readonly IRSRuntimeEntity m_Entity;

        private RSRuleTableData m_Table;

        private Routine[] m_Routines;
        private RuleState[] m_States;

        public RSRuntimeRuleTable(IRSRuntimeEntity inOwner)
        {
            m_Entity = inOwner;
        }

        public void Initialize(RSRuleTableData inTable)
        {
            Data = inTable;
        }

        public void Destroy()
        {
            Data = null;
        }

        public RSRuleTableData Data
        {
            get { return m_Table; }
            set
            {
                AssignTable(value);
            }
        }

        private void AssignTable(RSRuleTableData inTable)
        {
            if (m_Table == inTable)
                return;

            if (m_Table != null)
            {
                m_Entity.Manager?.DeregisterTriggers(m_Entity, m_Table.UniqueTriggers);
            }

            m_Table = inTable;
            StopAll();

            if (m_Table != null)
            {
                int length = m_Table.Rules != null ? m_Table.Rules.Length : 0;
                Array.Resize(ref m_States, length);
                Array.Resize(ref m_Routines, length);

                for (int i = 0; i < length; ++i)
                {
                    RSRuleData rule = m_Table.Rules[i];
                    m_States[i] = rule.Enabled ? 0 : RuleState.Disabled;
                }

                m_Entity.Manager?.RegisterTriggers(m_Entity, m_Table.UniqueTriggers);
            }
            else
            {
                m_States = null;
                m_Routines = null;
            }
        }

        public event TriggerDelegate OnTrigger;

        #region Operations

        /// <summary>
        /// Enables all rules with the given name.
        /// </summary>
        public void EnableRule(string inRuleName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.Name, inRuleName))
                    m_States[i] &= ~RuleState.Disabled;
            }
        }

        /// <summary>
        /// Disables all rules with the given name.
        /// </summary>
        public void DisableRule(string inRuleName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.Name, inRuleName))
                    m_States[i] |= RuleState.Disabled;
            }
        }

        /// <summary>
        /// Stops execution of all rules with the given name.
        /// </summary>
        public void StopRule(string inRuleName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.Name, inRuleName))
                    m_Routines[i].Stop();
            }
        }

        /// <summary>
        /// Enables all rules with the given group.
        /// </summary>
        public void EnableRuleGroup(string inGroupName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.RoutineGroup, inGroupName))
                    m_States[i] &= ~RuleState.Disabled;
            }
        }

        /// <summary>
        /// Disables all rules with the given group.
        /// </summary>
        public void DisableRuleGroup(string inGroupName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.RoutineGroup, inGroupName))
                    m_States[i] |= RuleState.Disabled;
            }
        }

        /// <summary>
        /// Stops execution of all rules with the given group.
        /// </summary>
        public void StopRuleGroup(string inGroupName)
        {
            RSRuleData[] rules = m_Table?.Rules;
            if (rules == null || rules.Length <= 0)
                return;

            for (int i = 0; i < rules.Length; ++i)
            {
                RSRuleData rule = rules[i];
                if (ScriptUtils.StringMatch(rule.RoutineGroup, inGroupName))
                    m_Routines[i].Stop();
            }
        }

        /// <summary>
        /// Enables all rules.
        /// </summary>
        public void EnableAll()
        {
            if (m_States != null)
            {
                for (int i = 0; i < m_States.Length; ++i)
                {
                    m_States[i] &= ~RuleState.Disabled;
                }
            }
        }

        /// <summary>
        /// Disables all rules.
        /// </summary>
        public void DisableAll()
        {
            if (m_States != null)
            {
                for (int i = 0; i < m_States.Length; ++i)
                {
                    m_States[i] |= RuleState.Disabled;
                }
            }
        }

        /// <summary>
        /// Stops execution of all rules.
        /// </summary>
        public void StopAll()
        {
            if (m_Routines != null)
            {
                for (int i = 0; i < m_Routines.Length; ++i)
                {
                    m_Routines[i].Stop();
                }
            }
        }

        /// <summary>
        /// Resets all rules.
        /// </summary>
        public void ResetAll()
        {
            StopAll();

            if (m_Table != null)
            {
                int length = m_Table.Rules != null ? m_Table.Rules.Length : 0;
                for (int i = 0; i < length; ++i)
                {
                    RSRuleData rule = m_Table.Rules[i];
                    m_States[i] = rule.Enabled ? 0 : RuleState.Disabled;
                }
            }
        }

        #endregion // Operations

        internal void EvaluateTrigger(RSTriggerId inTriggerId, ExecutionScope inScope)
        {
            if (!m_Entity.IsAlive())
                return;

            if (OnTrigger != null)
            {
                object arg = RSInterop.ToObject(inScope.Argument, inScope);
                OnTrigger.Invoke(inTriggerId, arg);
            }

            RSRuleData[] rules = m_Table?.Rules;
            int ruleCount;
            if (rules == null || (ruleCount = rules.Length) <= 0)
                return;

            using(PooledSet<string> triggeredGroups = PooledSet<string>.Alloc())
            {
                for (int i = 0; i < ruleCount; ++i)
                {
                    RSRuleData rule = rules[i];
                    if (rule.TriggerId != inTriggerId)
                        continue;

                    if (m_States[i].HasFlag(RuleState.Disabled))
                        continue;

                    if (rule.DontInterrupt && m_Routines[i])
                        continue;

                    if (!inScope.EvaluateConditions(rule.Conditions, rule.ConditionSubset))
                        continue;

                    if (!string.IsNullOrEmpty(rule.RoutineGroup))
                    {
                        if (!triggeredGroups.Add(rule.RoutineGroup))
                            continue;

                        StopRuleGroup(rule.RoutineGroup);
                    }

                    if (rule.OnlyOnce)
                        m_States[i] |= RuleState.Disabled;

                    if (rule.Actions != null)
                    {
                        ExecutionScope scope = inScope;
                        scope.m_Environment.CloneScopeIfNecessary(scope, rule.Flags, out scope);

                        m_Routines[i].Replace(m_Entity.ProxyObject, scope.PerformActions(rule.Actions))
                            .ExecuteWhileDisabled().SetPhase(m_Entity.ExecutionPhase)
                            .TryManuallyUpdate(0);
                    }
                }
            }
        }

        #region Persist

        internal void Persist(ref RSPersistRuleTableData ioRuleTableData)
        {
            if (m_Table == null)
                return;

            if (ioRuleTableData == null)
            {
                ioRuleTableData = new RSPersistRuleTableData();
            }

            // TODO(Beau): Store table id?

            Array.Resize(ref ioRuleTableData.Rules, m_States.Length);
            for (int i = 0; i < m_States.Length; ++i)
            {
                ref var data = ref ioRuleTableData.Rules[i];
                data.Id = m_Table.Rules[i].Id;
                data.State = m_States[i];
            }
        }

        internal void Restore(RSPersistRuleTableData inRuleTableData)
        {
            if (inRuleTableData == null)
                return;

            // TODO(Beau): Restore rule table from id?

            if (inRuleTableData.Rules != null)
            {
                for (int i = 0; i < inRuleTableData.Rules.Length; ++i)
                {
                    ref var data = ref inRuleTableData.Rules[i];
                    int idxInTable = TableUtils.IndexOfRule(m_Table, data.Id);
                    if (idxInTable >= 0)
                    {
                        m_States[idxInTable] = data.State;
                    }
                }
            }
        }

        #endregion // Persist
    }
}