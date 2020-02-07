using System.Collections.Generic;
using BeauData;
using BeauUtil;

namespace RuleScript.Data
{
    public sealed class RSPersistEntityData : ISerializedObject, ISerializedVersion, ICopyCloneable<RSPersistEntityData>
    {
        public RSEntityId EntityId;
        public bool Active;
        public RSPersistComponentData[] ComponentData;
        public RSPersistRuleTableData TableData;
        public IRSCustomPersistData CustomData;

        public RSPersistComponentData FindComponentData(int inComponentType)
        {
            if (ComponentData != null)
            {
                for (int i = ComponentData.Length - 1; i >= 0; --i)
                {
                    if (ComponentData[i].ComponentType == inComponentType)
                    {
                        return ComponentData[i];
                    }
                }
            }

            return null;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 2; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Int32Proxy("id", ref EntityId, FieldOptions.PreferAttribute);
            ioSerializer.Serialize("active", ref Active, FieldOptions.PreferAttribute);
            ioSerializer.ObjectArray("componentData", ref ComponentData);
            if (ioSerializer.ObjectVersion >= 2)
            {
                ioSerializer.Object("table", ref TableData);
            }
            ioSerializer.Object("customData", ref CustomData, FieldOptions.Optional);
        }

        #endregion // ISerializedObject

        #region ICopyCloneable

        public RSPersistEntityData Clone()
        {
            RSPersistEntityData clone = new RSPersistEntityData();
            clone.EntityId = EntityId;
            clone.Active = Active;
            clone.ComponentData = CloneUtils.DeepClone(ComponentData);
            clone.TableData = CloneUtils.Clone(TableData);
            clone.CustomData = CloneUtils.Clone(CustomData);
            return clone;
        }

        public void CopyFrom(RSPersistEntityData inClone)
        {
            EntityId = inClone.EntityId;
            Active = inClone.Active;
            CloneUtils.CopyFrom(ref ComponentData, inClone.ComponentData);
            CloneUtils.CopyFrom(ref TableData, inClone.TableData);
            CloneUtils.CopyFrom(ref CustomData, inClone.CustomData);
        }

        #endregion // ICopyCloneable
    }
}