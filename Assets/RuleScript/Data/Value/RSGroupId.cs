using System;
using BeauData;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Group id.
    /// </summary>
    [Serializable]
    public struct RSGroupId : ISerializedObject, ISerializedVersion, IEquatable<RSGroupId>, ISerializedProxy<Int32>
    {
        [SerializeField]
        private int m_Value;

        public RSGroupId(int inValue)
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

        public bool Equals(RSGroupId other)
        {
            return m_Value == other.m_Value;
        }
        
        #endregion // IEquatable
        
        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is RSGroupId)
                return Equals((RSGroupId) obj);
            return false;
        }

        public override int GetHashCode()
        {
            return m_Value;
        }

        static public bool operator==(RSGroupId a, RSGroupId b)
        {
            return a.Equals(b);
        }

        static public bool operator!=(RSGroupId a, RSGroupId b)
        {
            return !a.Equals(b);
        }

        static public explicit operator int(RSGroupId inGroup)
        {
            return inGroup.m_Value;
        }

        public override string ToString()
        {
            if (m_Value == 0)
                return string.Empty;
            
            return string.Format("[Group {0}]", m_Value);
        }

        public string ToString(RSLibrary inLibrary)
        {
            string realName = inLibrary?.GetGroup(m_Value)?.Name;
            return realName ?? ToString();
        }

        #endregion // Overrides
        
        #region Static

        static private RSGroupId s_Null = default(RSGroupId);

        static public RSGroupId Null { get { return s_Null; } }

        #endregion // Static
    }
}