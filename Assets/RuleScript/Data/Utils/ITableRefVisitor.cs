using System;
using System.Collections.Generic;
using BeauData;

namespace RuleScript.Data
{
    /// <summary>
    /// Visits data elements in a data and collects references.
    /// </summary>
    public interface ITableRefVisitor
    {
        void Visit(IRSRuleTableSource inRuleTableSource);
        void Visit(RSRuleTableData inRuleTableData, TableLineRef inSourceRef);
        void Visit(RSRuleData inRuleData, TableLineRef inSourceRef);
        void Visit(RSConditionData inConditionData, TableLineRef inSourceRef);
        void Visit(RSActionData inActionData, TableLineRef inSourceRef);
        void Visit(RSResolvableValueData inResolveableValueData, TableLineRef inSourceRef);
        void Visit(NestedValue inNestedValue, TableLineRef inSourceRef);
        void Visit(RSValue inValue, TableLineRef inSourceRef);
        void Visit(EntityScopedIdentifier inScopedIdentifier, EntityScopedIdentifier.Type inType, TableLineRef inSourceRef);
        void Visit(EntityScopeData inScope, TableLineRef inSourceRef);
    }

    /// <summary>
    /// Abstract implementation of a table data visitor.
    /// Automatically traverses subelements.
    /// </summary>
    public abstract class AbstractTableRefVisitor : ITableRefVisitor
    {
        #region Refs

        protected readonly List<TableLineRef> m_CollectedRefs = new List<TableLineRef>();

        public IList<TableLineRef> CollectedRefs { get { return m_CollectedRefs; } }

        protected void AddRef(TableLineRef inRef)
        {
            if (!m_CollectedRefs.Contains(inRef))
            {
                m_CollectedRefs.Add(inRef);
            }
        }

        protected void AddRef(TableLineRef inRef, string inDescriptor)
        {
            AddRef(inRef.WithDescriptor(inDescriptor));
        }

        #endregion // Refs

        public virtual void Visit(IRSRuleTableSource inRuleTableSource)
        {
            if (inRuleTableSource == null)
            {
                return;
            }

            Visit(inRuleTableSource.TableData, TableLineRef.FromTable(inRuleTableSource));
        }

        public virtual void Visit(RSRuleTableData inRuleTableData, TableLineRef inSourceRef)
        {
            if (inRuleTableData == null)
            {
                return;
            }

            if (inRuleTableData.Rules != null)
            {
                for (int i = 0; i < inRuleTableData.Rules.Length; ++i)
                {
                    Visit(inRuleTableData.Rules[i], inSourceRef);
                }
            }
        }

        public virtual void Visit(RSRuleData inRuleData, TableLineRef inSourceRef)
        {
            if (inRuleData == null)
            {
                return;
            }

            inSourceRef = inSourceRef.WithRule(inRuleData.Id).CombineEnabled(inRuleData.Enabled);

            if (inRuleData.Conditions != null)
            {
                for (int i = 0; i < inRuleData.Conditions.Length; ++i)
                {
                    Visit(inRuleData.Conditions[i], inSourceRef.WithCondition(i));
                }
            }

            if (inRuleData.Actions != null)
            {
                for (int i = 0; i < inRuleData.Actions.Length; ++i)
                {
                    Visit(inRuleData.Actions[i], inSourceRef.WithAction(i));
                }
            }
        }

        public virtual void Visit(RSConditionData inConditionData, TableLineRef inSourceRef)
        {
            if (inConditionData == null)
                return;

            inSourceRef = inSourceRef.CombineEnabled(inConditionData.Enabled);

            Visit(inConditionData.Query, inSourceRef);
            Visit(inConditionData.Target, inSourceRef);
        }

        public virtual void Visit(RSActionData inActionData, TableLineRef inSourceRef)
        {
            if (inActionData == null)
                return;

            inSourceRef = inSourceRef.CombineEnabled(inActionData.Enabled);

            Visit(inActionData.Action, EntityScopedIdentifier.Type.Action, inSourceRef);
            if (inActionData.Arguments != null)
            {
                for (int i = 0; i < inActionData.Arguments.Length; ++i)
                {
                    Visit(inActionData.Arguments[i], inSourceRef);
                }
            }
        }

        public virtual void Visit(RSResolvableValueData inResolvableValueData, TableLineRef inSourceRef)
        {
            switch (inResolvableValueData.Mode)
            {
                case ResolvableValueMode.Value:
                    {
                        Visit(inResolvableValueData.Value, inSourceRef);
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        Visit(inResolvableValueData.Query, EntityScopedIdentifier.Type.Query, inSourceRef);
                        if (inResolvableValueData.QueryArguments != null)
                        {
                            for (int i = 0; i < inResolvableValueData.QueryArguments.Length; ++i)
                            {
                                Visit(inResolvableValueData.QueryArguments[i], inSourceRef);
                            }
                        }
                        break;
                    }
            }
        }

        public virtual void Visit(NestedValue inNestedValue, TableLineRef inSourceRef)
        {
            switch (inNestedValue.Mode)
            {
                case ResolvableValueMode.Value:
                    {
                        Visit(inNestedValue.Value, inSourceRef);
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        Visit(inNestedValue.Query, EntityScopedIdentifier.Type.Query, inSourceRef);
                        break;
                    }
            }
        }

        public virtual void Visit(RSValue inValue, TableLineRef inSourceRef)
        {
            if (inValue.GetInnerType() == RSValue.InnerType.EntityScope)
            {
                Visit(inValue.AsEntity, inSourceRef);
            }
        }

        public virtual void Visit(EntityScopedIdentifier inScopedIdentifier, EntityScopedIdentifier.Type inType, TableLineRef inSourceRef)
        {
            Visit(inScopedIdentifier.Scope, inSourceRef);
        }

        public virtual void Visit(EntityScopeData inScope, TableLineRef inSourceRef) { }
    }
}