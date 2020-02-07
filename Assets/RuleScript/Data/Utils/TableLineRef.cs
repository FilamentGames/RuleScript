using System;
using System.Collections.Generic;

namespace RuleScript.Data
{
    public struct TableLineRef : IEquatable<TableLineRef>
    {
        public readonly IRSRuleTableSource TableSource;
        public readonly string RuleId;
        public readonly int ConditionIndex;
        public readonly int ActionIndex;
        public readonly int ElementIndex;

        public readonly bool Enabled;
        public readonly string Descriptor;
        public readonly object UserData;

        private TableLineRef(IRSRuleTableSource inSource, string inRuleId, int inConditionIndex, int inActionIndex, int inElementIndex, bool inbEnabled, string inDescriptor, object inUserData)
        {
            TableSource = inSource;
            RuleId = inRuleId;
            ConditionIndex = inConditionIndex;
            ActionIndex = inActionIndex;
            ElementIndex = inElementIndex;
            Enabled = inbEnabled;
            Descriptor = inDescriptor;
            UserData = inUserData;
        }

        #region Modifications

        public TableLineRef WithRule(string inRuleId)
        {
            return new TableLineRef(TableSource, inRuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled, Descriptor, UserData);
        }

        public TableLineRef WithCondition(int inConditionIndex)
        {
            return new TableLineRef(TableSource, RuleId, inConditionIndex, -1, ElementIndex, Enabled, Descriptor, UserData);
        }

        public TableLineRef WithAction(int inActionIndex)
        {
            return new TableLineRef(TableSource, RuleId, -1, inActionIndex, ElementIndex, Enabled, Descriptor, UserData);
        }

        public TableLineRef WithElement(int inElementIndex)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, inElementIndex, Enabled, Descriptor, UserData);
        }

        public TableLineRef WithDescriptor(string inDescriptor)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled, inDescriptor, UserData);
        }

        public TableLineRef WithDescriptor(string inDescriptor, object inArgument)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled, string.Format(inDescriptor, inArgument), UserData);
        }

        public TableLineRef WithDescriptor(string inDescriptor, params object[] inArguments)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled, string.Format(inDescriptor, inArguments), UserData);
        }

        public TableLineRef WithUserData(object inUserData)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled, Descriptor, inUserData);
        }

        public TableLineRef WithEnabled(bool inbEnabled)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, inbEnabled, Descriptor, UserData);
        }

        public TableLineRef CombineEnabled(bool inbEnabled)
        {
            return new TableLineRef(TableSource, RuleId, ConditionIndex, ActionIndex, ElementIndex, Enabled && inbEnabled, Descriptor, UserData);
        }

        #endregion // Modifications

        #region Statics

        static public TableLineRef FromTable(IRSRuleTableSource inSource)
        {
            return FromTable(inSource, inSource?.ToString() ?? string.Empty);
        }

        static public TableLineRef FromTable(IRSRuleTableSource inSource, string inName)
        {
            return new TableLineRef(inSource, inName, -1, -1, -1, true, null, null);
        }

        #endregion // Statics

        #region IEquatable

        public bool Equals(TableLineRef other)
        {
            return TableSource == other.TableSource &&
                RuleId == other.RuleId &&
                ConditionIndex == other.ConditionIndex &&
                ActionIndex == other.ActionIndex &&
                Enabled == other.Enabled &&
                Descriptor == other.Descriptor &&
                UserData == other.UserData;
        }

        #endregion // IEquatable
        
        // TODO(Beau): Add compare operation?

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is TableLineRef)
            {
                return Equals((TableLineRef) obj);
            }

            return false;
        }

        public override int GetHashCode()
        {
            int hash = RuleId.GetHashCode();
            hash = (hash << 5) ^ ConditionIndex.GetHashCode();
            hash = (hash >> 3) ^ ActionIndex.GetHashCode();
            hash = (hash << 2) ^ ElementIndex.GetHashCode();
            hash = (hash << 4) ^ Enabled.GetHashCode();
            if (TableSource != null)
                hash = (hash << 7) ^ TableSource.GetHashCode();
            if (Descriptor != null)
                hash = (hash >> 4) ^ Descriptor.GetHashCode();
            if (UserData != null)
                hash = (hash << 3) ^ UserData.GetHashCode();
            return hash;
        }

        static public bool operator ==(TableLineRef lhs, TableLineRef rhs)
        {
            return lhs.Equals(rhs);
        }

        static public bool operator !=(TableLineRef lhs, TableLineRef rhs)
        {
            return !lhs.Equals(rhs);
        }

        #endregion // Overrides
    }
}