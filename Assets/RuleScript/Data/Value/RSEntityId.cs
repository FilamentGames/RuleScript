using System;
using BeauData;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Entity id.
    /// </summary>
    [Serializable]
    public struct RSEntityId : ISerializedObject, ISerializedVersion, IEquatable<RSEntityId>, ISerializedProxy<Int32>
    {
        [SerializeField]
        private int m_Value;

        public RSEntityId(int inValue)
        {
            m_Value = inValue;
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Serialize("value", ref m_Value, 0);
        }

        #endregion // ISerializedObject

        #region ISerializedProxy

        int ISerializedProxy<int>.GetProxyValue(ISerializerContext inContext)
        {
            return m_Value;
        }

        void ISerializedProxy<int>.SetProxyValue(int inValue, ISerializerContext inContext)
        {
            m_Value = inValue;
        }

        #endregion // ISerializedProxy

        #region IEquatable

        public bool Equals(RSEntityId other)
        {
            return m_Value == other.m_Value;
        }

        #endregion // IEquatable

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is RSEntityId)
                return Equals((RSEntityId) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return m_Value;
        }

        static public bool operator ==(RSEntityId a, RSEntityId b)
        {
            return a.Equals(b);
        }

        static public bool operator !=(RSEntityId a, RSEntityId b)
        {
            return !a.Equals(b);
        }

        static public explicit operator int(RSEntityId inGroup)
        {
            return inGroup.m_Value;
        }

        public override string ToString()
        {
            if (m_Value == 0)
                return string.Empty;

            return string.Format("[Entity {0}]", m_Value);
        }

        #endregion // Overrides

        #region Static

        static private readonly RSEntityId s_Null = default(RSEntityId);
        static private readonly RSEntityId s_Invalid = new RSEntityId(int.MinValue);

        static public RSEntityId Null { get { return s_Null; } }
        static public RSEntityId Invalid { get { return s_Invalid; } }

        private const int ID_INDEX_MASK = 0x00FFFFFF;
        private const int ID_FLAGS_SHIFT = 24;

        static public RSEntityId GenerateId(int inIndex, byte inFlags)
        {
            int id = (inIndex & ID_INDEX_MASK) | (inFlags << ID_FLAGS_SHIFT);
            return new RSEntityId(id);
        }

        #endregion // Static
    }
}