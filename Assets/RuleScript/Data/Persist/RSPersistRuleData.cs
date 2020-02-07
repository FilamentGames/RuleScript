using System.Collections.Generic;
using BeauData;
using RuleScript.Runtime;
using BeauUtil;

namespace RuleScript.Data
{
    public struct RSPersistRuleData : ISerializedObject, ISerializedVersion
    {
        public string Id;
        internal RSRuntimeRuleTable.RuleState State;

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("id", ref Id, FieldOptions.PreferAttribute);
            ioSerializer.Enum("state", ref State, FieldOptions.PreferAttribute);
        }

        #endregion // ISerializedObject
    }
}