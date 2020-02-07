using BeauData;

namespace RuleScript.Data
{
    public struct RSNamedValue : ISerializedObject, ISerializedVersion
    {
        public string Name;
        public RSValue Value;

        public RSNamedValue(string inName, RSValue inValue)
        {
            Name = inName;
            Value = inValue;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("name", ref Name);
            ioSerializer.Object("value", ref Value);
        }

        #endregion // ISerializedObject
    }
}