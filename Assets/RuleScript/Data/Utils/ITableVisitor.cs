using System;
using BeauData;

namespace RuleScript.Data
{
    /// <summary>
    /// Visits data elements within a table.
    /// </summary>
    public interface ITableVisitor
    {
        bool Visit(IRSRuleTableSource ioRuleTableSource);
        bool Visit(RSRuleTableData ioRuleTable);
        bool Visit(RSRuleData ioRuleData);
        bool Visit(RSConditionData ioConditionData);
        bool Visit(RSActionData ioActionData);
        bool Visit(RSResolvableValueData ioResolvableValueData);
        bool Visit(ref NestedValue ioNestedValue);
        bool Visit(ref RSValue ioValue);
        bool Visit(ref EntityScopedIdentifier ioScopedIdentifier, EntityScopedIdentifier.Type inType);
        bool Visit(ref EntityScopeData ioScope);
    }

    /// <summary>
    /// Abstract implementation of a table data visitor.
    /// Automatically traverses subelements.
    /// </summary>
    public abstract class AbstractTableVisitor : ITableVisitor
    {
        public virtual bool Visit(IRSRuleTableSource ioRuleTableSource)
        {
            if (ioRuleTableSource == null)
            {
                return false;
            }

            return Visit(ioRuleTableSource.TableData);
        }

        public virtual bool Visit(RSRuleTableData ioRuleTable)
        {
            if (ioRuleTable == null)
            {
                return false;
            }

            bool bChanged = false;

            if (ioRuleTable.Rules != null)
            {
                for (int i = 0; i < ioRuleTable.Rules.Length; ++i)
                {
                    bChanged |= Visit(ioRuleTable.Rules[i]);
                }
            }

            return bChanged;
        }

        public virtual bool Visit(RSRuleData ioRuleData)
        {
            if (ioRuleData == null)
            {
                return false;
            }

            bool bChanged = false;

            if (ioRuleData.Conditions != null)
            {
                for (int i = 0; i < ioRuleData.Conditions.Length; ++i)
                {
                    bChanged |= Visit(ioRuleData.Conditions[i]);
                }
            }

            if (ioRuleData.Actions != null)
            {
                for (int i = 0; i < ioRuleData.Actions.Length; ++i)
                {
                    bChanged |= Visit(ioRuleData.Actions[i]);
                }
            }

            return bChanged;
        }

        public virtual bool Visit(RSConditionData ioConditionData)
        {
            if (ioConditionData == null)
                return false;

            bool bChanged = Visit(ioConditionData.Query);
            bChanged |= Visit(ioConditionData.Target);
            return bChanged;
        }

        public virtual bool Visit(RSActionData ioActionData)
        {
            if (ioActionData == null)
                return false;

            bool bChanged = Visit(ref ioActionData.Action, EntityScopedIdentifier.Type.Action);
            if (ioActionData.Arguments != null)
            {
                for (int i = 0; i < ioActionData.Arguments.Length; ++i)
                {
                    Visit(ioActionData.Arguments[i]);
                }
            }
            return bChanged;
        }

        public virtual bool Visit(RSResolvableValueData ioResolvableValueData)
        {
            bool bChanged = false;

            switch (ioResolvableValueData.Mode)
            {
                case ResolvableValueMode.Value:
                    {
                        bChanged |= Visit(ref ioResolvableValueData.Value);
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        bChanged |= Visit(ref ioResolvableValueData.Query, EntityScopedIdentifier.Type.Query);
                        if (ioResolvableValueData.QueryArguments != null)
                        {
                            for (int i = 0; i < ioResolvableValueData.QueryArguments.Length; ++i)
                            {
                                bChanged |= Visit(ref ioResolvableValueData.QueryArguments[i]);
                            }
                        }
                        break;
                    }
            }

            return bChanged;
        }

        public virtual bool Visit(ref NestedValue ioNestedValue)
        {
            bool bChanged = false;

            switch (ioNestedValue.Mode)
            {
                case ResolvableValueMode.Value:
                    {
                        RSValue val = ioNestedValue.Value;
                        if (Visit(ref val))
                        {
                            bChanged = true;
                            ioNestedValue = NestedValue.FromValue(val);
                        }
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        EntityScopedIdentifier query = ioNestedValue.Query;
                        if (Visit(ref query, EntityScopedIdentifier.Type.Query))
                        {
                            bChanged = true;
                            ioNestedValue = NestedValue.FromQuery(query);
                        }
                        break;
                    }
            }

            return bChanged;
        }

        public virtual bool Visit(ref RSValue ioValue)
        {
            if (ioValue.GetInnerType() == RSValue.InnerType.EntityScope)
            {
                EntityScopeData scope = ioValue.AsEntity;
                if (Visit(ref scope))
                {
                    ioValue = RSValue.FromEntity(scope);
                    return true;
                }
            }

            return false;
        }

        public virtual bool Visit(ref EntityScopedIdentifier ioScopedIdentifier, EntityScopedIdentifier.Type inType)
        {
            EntityScopeData scope = ioScopedIdentifier.Scope;
            if (Visit(ref scope))
            {
                ioScopedIdentifier = new EntityScopedIdentifier(scope, ioScopedIdentifier.Id);
                return true;
            }

            return false;
        }

        public virtual bool Visit(ref EntityScopeData ioScope) { return false; }
    }
}