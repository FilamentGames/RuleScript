using System;
using System.Runtime.InteropServices;
using BeauData;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Trigger id.
    /// </summary>
    [Serializable, StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct RSTriggerId : ISerializedObject, ISerializedVersion, IEquatable<RSTriggerId>, ISerializedProxy<Int32>
    {
        [FieldOffset(0), SerializeField]
        private int m_Value;

        public RSTriggerId(int inValue)
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

        public bool Equals(RSTriggerId other)
        {
            return m_Value == other.m_Value;
        }

        #endregion // IEquatable

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is RSTriggerId)
                return Equals((RSTriggerId) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return m_Value;
        }

        static public bool operator ==(RSTriggerId a, RSTriggerId b)
        {
            return a.Equals(b);
        }

        static public bool operator !=(RSTriggerId a, RSTriggerId b)
        {
            return !a.Equals(b);
        }

        static public explicit operator int(RSTriggerId inTrigger)
        {
            return inTrigger.m_Value;
        }

        public override string ToString()
        {
            if (m_Value == 0)
                return string.Empty;

            return string.Format("[Trigger {0}]", m_Value);
        }

        public string ToString(RSLibrary inLibrary)
        {
            string realName = inLibrary?.GetTrigger(m_Value)?.Name;
            return realName ?? ToString();
        }

        #endregion // Overrides

        #region Static

        static private RSTriggerId s_Null = default(RSTriggerId);

        static public RSTriggerId Null { get { return s_Null; } }

        #endregion // Static
    }
}