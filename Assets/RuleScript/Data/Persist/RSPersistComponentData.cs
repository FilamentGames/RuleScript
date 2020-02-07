using System.Collections.Generic;
using BeauData;
using BeauUtil;

namespace RuleScript.Data
{
    public sealed class RSPersistComponentData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSPersistComponentData>
    {
        public int ComponentType;
        public RSNamedValue[] NamedValues;
        public IRSCustomPersistData CustomData;

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("componentType", ref ComponentType, FieldOptions.PreferAttribute);
            ioSerializer.ObjectArray("fields", ref NamedValues);
            ioSerializer.Object("customData", ref CustomData, FieldOptions.Optional);
        }

        #endregion // ISerializedObject

        #region ICopyCloneable

        RSPersistComponentData ICopyCloneable<RSPersistComponentData>.Clone()
        {
            RSPersistComponentData clone = new RSPersistComponentData();
            clone.ComponentType = ComponentType;
            clone.NamedValues = CloneUtils.Clone(NamedValues);
            clone.CustomData = CloneUtils.Clone(CustomData);
            return clone;
        }

        void ICopyCloneable<RSPersistComponentData>.CopyFrom(RSPersistComponentData inClone)
        {
            ComponentType = inClone.ComponentType;
            CloneUtils.CopyFrom(ref NamedValues, inClone.NamedValues);
            CloneUtils.CopyFrom(ref CustomData, inClone.CustomData);
        }

        #endregion // ICopyCloneable
    }
}