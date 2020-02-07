using System;
using System.Collections.Generic;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    static public class TableUtils
    {
        #region Unique Triggers

        /// <summary>
        /// Outputs all unique triggers for the given table.
        /// </summary>
        static public int UniqueRuleTriggers(RSRuleTableData inTableData, ICollection<RSTriggerId> outCollection)
        {
            Assert.True(inTableData != null, "Cannot read from null table");
            Assert.True(outCollection != null, "Cannot output to null collection");

            if (inTableData == null || inTableData.Rules == null)
                return 0;

            int count = 0;
            for (int i = 0; i < inTableData.Rules.Length; ++i)
            {
                RSTriggerId trigger = inTableData.Rules[i].TriggerId;
                if (trigger == RSTriggerId.Null)
                    continue;
                if (outCollection.Contains(trigger))
                    continue;

                outCollection.Add(trigger);
                ++count;
            }

            return count;
        }

        /// <summary>
        /// Updates the array of all unique triggers for the given table.
        /// </summary>
        static public void UpdateUniqueRuleTriggers(RSRuleTableData ioTable)
        {
            HashSet<RSTriggerId> uniqueTriggers = new HashSet<RSTriggerId>();
            int uniqueTriggerCount = TableUtils.UniqueRuleTriggers(ioTable, uniqueTriggers);
            ioTable.UniqueTriggers = new RSTriggerId[uniqueTriggerCount];
            uniqueTriggers.CopyTo(ioTable.UniqueTriggers);
        }

        #endregion // Unique Triggers

        #region Flags

        /// <summary>
        /// Outputs all flags for the given rule.
        /// </summary>
        static public RuleFlags GetRuleFlags(RSRuleData inRuleData, RSLibrary inLibrary)
        {
            Assert.True(inRuleData != null, "Cannot read from null rule");

            RuleFlags flags = 0;

            if (inRuleData.Conditions != null)
            {
                foreach (var condition in inRuleData.Conditions)
                {
                    flags |= GetRuleFlags(condition, inLibrary);
                }
            }

            if (inRuleData.Actions != null)
            {
                foreach (var action in inRuleData.Actions)
                {
                    flags |= GetRuleFlags(action, inLibrary);
                }
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(EntityScopeData inScopeData, RSLibrary inLibrary)
        {
            if (inScopeData.Type == EntityScopeType.ObjectInRegister)
                return RuleFlags.UsesRegisters;

            return 0;
        }

        static private RuleFlags GetRuleFlags(RSActionData inActionData, RSLibrary inLibrary)
        {
            RuleFlags flags = 0;

            if (inActionData.Enabled)
            {
                flags |= GetRuleFlags(inActionData.Action.Scope, inLibrary);
                flags |= GetRuleFlags(inLibrary.GetAction(inActionData.Action.Id));

                if (inActionData.Arguments != null)
                {
                    foreach (var arg in inActionData.Arguments)
                    {
                        flags |= GetRuleFlags(arg, inLibrary);
                    }
                }
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(RSConditionData inConditionData, RSLibrary inLibrary)
        {
            RuleFlags flags = 0;

            if (inConditionData.Enabled)
            {
                flags |= GetRuleFlags(inConditionData.Query, inLibrary);
                if (inConditionData.Operator.IsBinary())
                {
                    flags |= GetRuleFlags(inConditionData.Target, inLibrary);
                }
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(RSResolvableValueData inValueData, RSLibrary inLibrary)
        {
            RuleFlags flags = 0;

            switch (inValueData.Mode)
            {
                case ResolvableValueMode.Query:
                    {
                        flags |= GetRuleFlags(inValueData.Query.Scope, inLibrary);
                        flags |= GetRuleFlags(inLibrary.GetQuery(inValueData.Query.Id));
                        if (inValueData.QueryArguments != null)
                        {
                            foreach (var arg in inValueData.QueryArguments)
                            {
                                flags |= GetRuleFlags(arg, inLibrary);
                            }
                        }
                        break;
                    }
                case ResolvableValueMode.Register:
                    {
                        flags |= RuleFlags.UsesRegisters;
                        break;
                    }
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(NestedValue inValueData, RSLibrary inLibrary)
        {
            RuleFlags flags = 0;

            switch (inValueData.Mode)
            {
                case ResolvableValueMode.Query:
                    {
                        flags |= GetRuleFlags(inValueData.Query.Scope, inLibrary);
                        flags |= GetRuleFlags(inLibrary.GetQuery(inValueData.Query.Id));
                        break;
                    }
                case ResolvableValueMode.Register:
                    {
                        flags |= RuleFlags.UsesRegisters;
                        break;
                    }
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(RSQueryInfo inInfo)
        {
            RuleFlags flags = 0;

            if (inInfo != null)
            {
                flags |= GetRuleFlags(inInfo.Flags);
            }

            return flags;
        }

        static private RuleFlags GetRuleFlags(RSActionInfo inInfo)
        {
            RuleFlags flags = 0;

            if (inInfo != null)
            {
                flags |= GetRuleFlags(inInfo.Flags);
            }

            return flags;
        }

        static public RuleFlags GetRuleFlags(RSMemberFlags inFlags)
        {
            RuleFlags flags = 0;

            if ((inFlags & RSMemberFlags.UsesRegisters) != 0)
                flags |= RuleFlags.UsesRegisters;

            return flags;
        }

        #endregion // Flags

        #region Updating Entity References

        /// <summary>
        /// Updates all references to an entity id within the given table.
        /// </summary>
        static public bool UpdateEntityRef(RSRuleTableData ioTableData, RSEntityId inEntityId, RSEntityId inNewId)
        {
            EntityRefUpdateVisitor updater = new EntityRefUpdateVisitor(inEntityId, inNewId);
            return updater.Visit(ioTableData);
        }

        /// <summary>
        /// Deletes all references to an entity id within the given table.
        /// </summary>
        static public bool DeleteEntityRef(RSRuleTableData ioTableData, RSEntityId inEntityId)
        {
            return UpdateEntityRef(ioTableData, inEntityId, RSEntityId.Invalid);
        }

        /// <summary>
        /// Visitor that updates entity id references.
        /// </summary>
        public class EntityRefUpdateVisitor : AbstractTableVisitor
        {
            private readonly RSEntityId m_OldId;
            private readonly RSEntityId m_NewId;

            public EntityRefUpdateVisitor(RSEntityId inOldId, RSEntityId inNewId)
            {
                m_OldId = inOldId;
                m_NewId = inNewId;
            }

            public override bool Visit(ref EntityScopeData ioScope)
            {
                if (ioScope.Type == EntityScopeType.ObjectById)
                {
                    if (ioScope.IdArg == m_OldId)
                    {
                        // UnityEngine.Debug.LogFormat("Updating entity id from {0} to {1}", m_OldId, m_NewId);
                        ioScope = EntityScopeData.Entity(m_NewId).WithLinks(ioScope.LinksArg, ioScope.UseFirstLink);
                        return true;
                    }
                }

                return false;
            }
        }

        #endregion // Updating Entity References

        #region Clean Up

        /// <summary>
        /// Attempts to clean up the given rule table.
        /// Returns if any modifications have been made.
        /// </summary>
        static public bool CleanUp(RSRuleTableData ioTableData)
        {
            if (ioTableData == null || ioTableData.Rules == null)
                return false;

            bool bChanged = false;
            HashSet<string> discoveredRuleIds = new HashSet<string>();
            for (int i = 0; i < ioTableData.Rules.Length; ++i)
            {
                bChanged |= CleanUp(ioTableData.Rules[i], discoveredRuleIds);
            }

            return bChanged;
        }

        static private bool CleanUp(RSRuleData ioRule, HashSet<string> ioDiscoveredRules)
        {
            bool bChanged = false;

            if (string.IsNullOrEmpty(ioRule.Id))
            {
                ioRule.Id = ScriptUtils.NewId();
                bChanged = true;
            }

            while (!ioDiscoveredRules.Add(ioRule.Id))
            {
                UnityEngine.Debug.LogErrorFormat("[TableUtils] Overlapping rule id {0}", ioRule.Id);
                ioRule.Id = ScriptUtils.NewId();
                bChanged = true;
            }

            return bChanged;
        }

        #endregion // Clean Up

        #region Rules

        /// <summary>
        /// Returns the index of the rule with the given id.
        /// </summary>
        static public int IndexOfRule(RSRuleTableData inTableData, string inRuleId)
        {
            if (inTableData == null || inTableData.Rules == null)
                return -1;

            if (string.IsNullOrEmpty(inRuleId))
                return -1;

            for (int i = 0; i < inTableData.Rules.Length; ++i)
            {
                if (inTableData.Rules[i].Id.Equals(inRuleId, System.StringComparison.Ordinal))
                    return i;
            }

            return -1;
        }

        #endregion // Rules

        #region References

        /// <summary>
        /// Locates references to an entity id.
        /// </summary>
        public class EntityIdRefVisitor : AbstractTableRefVisitor
        {
            private readonly RSEntityId m_EntityId;

            public EntityIdRefVisitor(RSEntityId inId)
            {
                m_EntityId = inId;
            }

            public override void Visit(EntityScopeData inScope, TableLineRef inSourceRef)
            {
                inSourceRef = inSourceRef.WithDescriptor("EntityScopeData");

                switch (inScope.Type)
                {
                    case EntityScopeType.ObjectById:
                        {
                            if (inScope.IdArg == m_EntityId)
                            {
                                AddRef(inSourceRef);
                            }
                            break;
                        }

                    case EntityScopeType.Invalid:
                        {
                            if (m_EntityId == RSEntityId.Invalid)
                            {
                                AddRef(inSourceRef);
                            }
                            break;
                        }

                    case EntityScopeType.Null:
                        {
                            if (m_EntityId == RSEntityId.Null)
                            {
                                AddRef(inSourceRef);
                            }
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Locates references to a trigger id.
        /// </summary>
        public class TriggerIdRefVisitor : AbstractTableRefVisitor
        {
            private readonly RSTriggerId m_TriggerId;

            public TriggerIdRefVisitor(RSTriggerId inId)
            {
                m_TriggerId = inId;
            }

            public override void Visit(RSRuleData inRuleData, TableLineRef inSourceRef)
            {
                base.Visit(inRuleData, inSourceRef);

                if (inRuleData != null)
                {
                    inSourceRef = inSourceRef.WithRule(inRuleData.Id).CombineEnabled(inRuleData.Enabled);

                    if (inRuleData.TriggerId == m_TriggerId)
                    {
                        AddRef(inSourceRef, "Rule Trigger");
                    }
                }
            }

            public override void Visit(RSValue inValue, TableLineRef inSourceRef)
            {
                switch (inValue.GetInnerType())
                {
                    case RSValue.InnerType.TriggerId:
                        {
                            if (inValue.AsTriggerId == m_TriggerId)
                            {
                                AddRef(inSourceRef);
                            }
                            break;
                        }

                    default:
                        {
                            base.Visit(inValue, inSourceRef);
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Locates references to a query id.
        /// </summary>
        public class QueryIdRefVisitor : AbstractTableRefVisitor
        {
            private readonly int m_QueryId;

            public QueryIdRefVisitor(int inId)
            {
                m_QueryId = inId;
            }

            public override void Visit(EntityScopedIdentifier inScopedIdentifier, EntityScopedIdentifier.Type inType, TableLineRef inSourceRef)
            {
                if (inType == EntityScopedIdentifier.Type.Query)
                {
                    if (inScopedIdentifier.Id == m_QueryId)
                    {
                        AddRef(inSourceRef);
                    }
                }

                base.Visit(inScopedIdentifier, inType, inSourceRef);
            }
        }

        /// <summary>
        /// Locates references to an action id.
        /// </summary>
        public class ActionIdRefVisitor : AbstractTableRefVisitor
        {
            private readonly int m_ActionId;

            public ActionIdRefVisitor(int inId)
            {
                m_ActionId = inId;
            }

            public override void Visit(EntityScopedIdentifier inScopedIdentifier, EntityScopedIdentifier.Type inType, TableLineRef inSourceRef)
            {
                if (inType == EntityScopedIdentifier.Type.Action)
                {
                    if (inScopedIdentifier.Id == m_ActionId)
                    {
                        AddRef(inSourceRef);
                    }
                }

                base.Visit(inScopedIdentifier, inType, inSourceRef);
            }
        }

        /// <summary>
        /// Locates references to a string.
        /// </summary>
        public class StringRefVisitor : AbstractTableRefVisitor
        {
            private readonly string m_String;

            public StringRefVisitor(string inString)
            {
                m_String = inString;
            }

            public override void Visit(RSRuleTableData inRuleTableData, TableLineRef inSourceRef)
            {
                base.Visit(inRuleTableData, inSourceRef);

                if (Match(inRuleTableData?.Name))
                {
                    AddRef(inSourceRef, "Rule Table Name: " + inRuleTableData.Name);
                }
            }

            public override void Visit(RSRuleData inRuleData, TableLineRef inSourceRef)
            {
                base.Visit(inRuleData, inSourceRef);

                if (inRuleData != null)
                {
                    inSourceRef = inSourceRef.WithRule(inRuleData.Id).CombineEnabled(inRuleData.Enabled);

                    if (Match(inRuleData.Name))
                    {
                        AddRef(inSourceRef, "Rule Name: " + inRuleData.Name);
                    }

                    if (Match(inRuleData.RoutineGroup))
                    {
                        AddRef(inSourceRef, "Rule Group: " + inRuleData.RoutineGroup);
                    }
                }
            }

            public override void Visit(RSValue inValue, TableLineRef inSourceRef)
            {
                base.Visit(inValue, inSourceRef);

                switch (inValue.GetInnerType())
                {
                    case RSValue.InnerType.Enum:
                    case RSValue.InnerType.String:
                        {
                            string asString = inValue.AsString;
                            if (Match(asString))
                            {
                                AddRef(inSourceRef, asString);
                            }
                            break;
                        }
                }
            }

            public override void Visit(EntityScopeData inScope, TableLineRef inSourceRef)
            {
                base.Visit(inScope, inSourceRef);

                switch (inScope.Type)
                {
                    case EntityScopeType.ObjectsWithName:
                        {
                            if (Match(inScope.SearchArg))
                            {
                                AddRef(inSourceRef, "Entity Name Search: " + inScope.SearchArg);
                            }
                            break;
                        }

                    case EntityScopeType.ObjectsWithPrefab:
                        {
                            if (Match(inScope.SearchArg))
                            {
                                AddRef(inSourceRef, "Entity Prefab Name Search " + inScope.SearchArg);
                            }
                            break;
                        }
                }

                if (Match(inScope.LinksArg))
                {
                    AddRef(inSourceRef, "Entity Link: " + inScope.LinksArg);
                }
            }

            private bool Match(string inString)
            {
                if (!string.IsNullOrEmpty(inString))
                {
                    return inString.IndexOf(m_String, StringComparison.OrdinalIgnoreCase) >= 0;
                }

                return false;
            }
        }

        #endregion // References
    }
}