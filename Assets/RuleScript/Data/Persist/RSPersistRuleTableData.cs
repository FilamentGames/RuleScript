using System.Collections.Generic;
using BeauData;
using BeauUtil;

namespace RuleScript.Data
{
    public sealed class RSPersistRuleTableData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSPersistRuleTableData>
    {
        public RSPersistRuleData[] Rules;

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.ObjectArray("rules", ref Rules);
        }

        #endregion // ISerializedObject

        #region ICopyCloneable

        RSPersistRuleTableData ICopyCloneable<RSPersistRuleTableData>.Clone()
        {
            RSPersistRuleTableData clone = new RSPersistRuleTableData();
            Rules = CloneUtils.Clone(clone.Rules);
            return clone;
        }

        void ICopyCloneable<RSPersistRuleTableData>.CopyFrom(RSPersistRuleTableData inClone)
        {
            CloneUtils.CopyFrom(ref Rules, inClone.Rules);
        }

        #endregion // ICopyCloneable
    }
}