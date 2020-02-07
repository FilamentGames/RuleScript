using System;
using BeauData;

namespace RuleScript.Data
{
    [Serializable]
    public struct RSEntityLinkData : ISerializedObject, ISerializedVersion
    {
        [RSEntitySelector] public RSEntityId EntityId;
        public string Name;

        public RSEntityLinkData(RSEntityId inEntity, string inName)
        {
            EntityId = inEntity;
            Name = inName;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("name", ref Name);
            ioSerializer.Int32Proxy("id", ref EntityId);
        }

        #endregion // ISerializedObject
    }
}