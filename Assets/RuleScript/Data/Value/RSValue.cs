using System;
using BeauData;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Data
{
    /// <summary>
    /// Generic value container.
    /// </summary>
    [Serializable]
    public struct RSValue : ISerializedObject, ISerializedVersion, IEquatable<RSValue>, IRSPreviewable
    {
        #region Types

        internal enum InnerType : byte
        {
            Null,

            Int32,
            Bool,
            Color,
            Float,
            Vector2,
            Vector3,
            Vector4,
            String,
            Enum,
            EntityScope,
            GroupId,
            TriggerId,

            Invalid = 255
        }

        #endregion // Types

        #region Inspector

        [SerializeField] private InnerType m_Type;

        [SerializeField] private int m_IntValue;
        [SerializeField] private float m_NumberValue;
        [SerializeField] private string m_StringValue;
        [SerializeField] private Vector4 m_VectorValue;
        [SerializeField] private string m_StringValue2;
        [SerializeField] private bool m_BoolValue2;

        #endregion // Inspector

        #region Constructors

        private RSValue(InnerType inType, int inIntValue = 0, float inNumberValue = 0f, string inStringValue = null, Vector4 inVectorValue = default(Vector4))
        {
            m_Type = inType;
            m_IntValue = inIntValue;
            m_NumberValue = inNumberValue;
            m_StringValue = inStringValue;
            m_VectorValue = inVectorValue;
            m_StringValue2 = null;
            m_BoolValue2 = false;
        }

        private RSValue(int inInt) : this(InnerType.Int32, inInt) { }

        private RSValue(bool inbBool) : this(InnerType.Bool, inbBool ? 1 : 0) { }

        private RSValue(float inFloat) : this(InnerType.Float, 0, inFloat) { }

        private RSValue(Color inColor) : this(InnerType.Color, 0, 0, null, inColor) { }

        private RSValue(Vector4 inVector) : this(InnerType.Vector4, 0, 0, null, inVector) { }

        private RSValue(Vector3 inVector) : this(InnerType.Vector3, 0, 0, null, inVector) { }

        private RSValue(Vector2 inVector) : this(InnerType.Vector2, 0, 0, null, inVector) { }

        private RSValue(string inString) : this(InnerType.String, 0, 0, inString) { }

        private RSValue(Enum inEnum) : this(InnerType.Enum, Convert.ToInt32(inEnum), 0, inEnum.GetType().AssemblyQualifiedName) { }

        private RSValue(EntityScopeData inScope) : this(InnerType.EntityScope)
        {
            EntityValue = inScope;
        }

        private RSValue(RSGroupId inGroup) : this(InnerType.GroupId, (int) inGroup) { }

        private RSValue(RSTriggerId inTrigger) : this(InnerType.TriggerId, (int) inTrigger) { }

        #endregion // Constructors

        #region Accessors

        public int AsInt
        {
            get { return IntValue; }
        }

        public bool AsBool
        {
            get { return BoolValue; }
        }

        public float AsFloat
        {
            get { return FloatValue; }
        }

        public Color AsColor
        {
            get { return ColorValue; }
        }

        public Vector2 AsVector2
        {
            get { return Vector2Value; }
        }

        public Vector3 AsVector3
        {
            get { return Vector3Value; }
        }

        public Vector4 AsVector4
        {
            get { return Vector4Value; }
        }

        public string AsString
        {
            get { return StringValue; }
        }

        public T AsEnum<T>() where T : Enum
        {
            return (T) AsEnum(typeof(T));
        }

        public object AsEnum(Type inType)
        {
            return Enum.ToObject(inType, IntValue);
        }

        public Enum AsEnum()
        {
            Type enumType = Type.GetType(m_StringValue);
            return (Enum) Enum.ToObject(enumType, m_IntValue);
        }

        public EntityScopeData AsEntity
        {
            get { return EntityValue; }
        }

        public RSGroupId AsGroupId
        {
            get { return GroupIdValue; }
        }

        public RSTriggerId AsTriggerId
        {
            get { return TriggerIdValue; }
        }

        #endregion // Accessors

        #region Value Types

        static private void ThrowInvalidCastException(InnerType inCurrentType, Type inDesiredType)
        {
            throw new InvalidCastException("Cannot cast from storage type " + inCurrentType + " to " + inDesiredType.FullName);
        }

        internal InnerType GetInnerType()
        {
            return m_Type;
        }

        /// <summary>
        /// Returns the C# type this RSValue represents.
        /// </summary>
        /// <returns></returns>
        public Type GetInteropType()
        {
            switch (m_Type)
            {
                case InnerType.Bool:
                    return typeof(bool);
                case InnerType.Color:
                    return typeof(Color);
                case InnerType.EntityScope:
                    return typeof(IRSEntity);
                case InnerType.Enum:
                    return Type.GetType(m_StringValue);
                case InnerType.Float:
                    return typeof(float);
                case InnerType.Int32:
                    return typeof(int);
                case InnerType.Invalid:
                    return null;
                case InnerType.Null:
                    return typeof(void);
                case InnerType.String:
                    return typeof(string);
                case InnerType.Vector2:
                    return typeof(Vector2);
                case InnerType.Vector3:
                    return typeof(Vector3);
                case InnerType.Vector4:
                    return typeof(Vector4);
                case InnerType.GroupId:
                    return typeof(RSGroupId);
                case InnerType.TriggerId:
                    return typeof(RSTriggerId);
                default:
                    throw new InvalidOperationException("Invalid type " + m_Type);
            }
        }

        private void SetInnerType(InnerType inType)
        {
            if (m_Type == inType)
                return;

            m_Type = inType;
            m_IntValue = 0;
            m_NumberValue = 0;
            m_VectorValue = default(Vector4);
            m_StringValue = null;
        }

        private int IntValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Int32:
                    case InnerType.Bool:
                    case InnerType.Enum:
                        return m_IntValue;

                    case InnerType.Float:
                        return (int) m_NumberValue;

                    case InnerType.Null:
                        return 0;

                    default:
                        ThrowInvalidCastException(m_Type, typeof(int));
                        return 0;
                }
            }
            set
            {
                SetInnerType(InnerType.Int32);
                m_IntValue = value;
            }
        }

        private float FloatValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Int32:
                    case InnerType.Bool:
                        return m_IntValue;

                    case InnerType.Float:
                        return m_NumberValue;

                    case InnerType.Null:
                        return 0;

                    default:
                        ThrowInvalidCastException(m_Type, typeof(float));
                        return 0;
                }
            }
            set
            {
                SetInnerType(InnerType.Float);
                m_NumberValue = value;
            }
        }

        private bool BoolValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Int32:
                    case InnerType.Bool:
                        return m_IntValue > 0;

                    case InnerType.Float:
                        return m_NumberValue > 0;

                    case InnerType.String:
                        return m_StringValue != null;

                    case InnerType.Null:
                        return false;

                    default:
                        ThrowInvalidCastException(m_Type, typeof(bool));
                        return false;
                }
            }
            set
            {
                SetInnerType(InnerType.Bool);
                m_IntValue = value ? 1 : 0;
            }
        }

        private Color ColorValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Color:
                    case InnerType.Vector2:
                    case InnerType.Vector3:
                    case InnerType.Vector4:
                        return (Color) m_VectorValue;

                    case InnerType.Null:
                        return default(Color);

                    default:
                        ThrowInvalidCastException(m_Type, typeof(Color));
                        return default(Color);
                }
            }
            set
            {
                SetInnerType(InnerType.Color);
                m_VectorValue = value;
            }
        }

        private Vector2 Vector2Value
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Color:
                    case InnerType.Vector2:
                    case InnerType.Vector3:
                    case InnerType.Vector4:
                        return m_VectorValue;

                    case InnerType.Null:
                        return default(Vector2);

                    default:
                        ThrowInvalidCastException(m_Type, typeof(Vector2));
                        return default(Vector2);
                }
            }
            set
            {
                SetInnerType(InnerType.Vector2);
                m_VectorValue = value;
            }
        }

        private Vector3 Vector3Value
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Color:
                    case InnerType.Vector2:
                    case InnerType.Vector3:
                    case InnerType.Vector4:
                        return m_VectorValue;

                    case InnerType.Null:
                        return default(Vector3);

                    default:
                        ThrowInvalidCastException(m_Type, typeof(Vector3));
                        return default(Vector3);
                }
            }
            set
            {
                SetInnerType(InnerType.Vector3);
                m_VectorValue = value;
            }
        }

        private Vector4 Vector4Value
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.Color:
                    case InnerType.Vector2:
                    case InnerType.Vector3:
                    case InnerType.Vector4:
                        return m_VectorValue;

                    case InnerType.Null:
                        return default(Vector4);

                    default:
                        ThrowInvalidCastException(m_Type, typeof(Vector4));
                        return default(Vector4);
                }
            }
            set
            {
                SetInnerType(InnerType.Vector4);
                m_VectorValue = value;
            }
        }

        private string StringValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.String:
                        return m_StringValue;
                    case InnerType.Color:
                        return ColorValue.ToString();
                    case InnerType.Bool:
                        return BoolValue ? "true" : "false";
                    case InnerType.Float:
                        return FloatValue.ToString();
                    case InnerType.Int32:
                        return IntValue.ToString();
                    case InnerType.Null:
                        return null;
                    case InnerType.Vector2:
                        return AsVector2.ToString();
                    case InnerType.Vector3:
                        return AsVector3.ToString();
                    case InnerType.Vector4:
                        return AsVector4.ToString();
                    case InnerType.EntityScope:
                        return EntityValue.ToString();
                    case InnerType.GroupId:
                        return GroupIdValue.ToString();
                    case InnerType.TriggerId:
                        return TriggerIdValue.ToString();
                    case InnerType.Enum:
                        {
                            Type enumType = Type.GetType(m_StringValue);
                            if (enumType == null)
                            {
                                Debug.LogErrorFormat("Enum type {0} no longer exists", m_StringValue);
                                return m_IntValue.ToString();
                            }
                            
                            try
                            {
                                return Enum.ToObject(enumType, m_IntValue).ToString();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                                Debug.LogErrorFormat("Enum {0} cannot be represented as type {1}", m_IntValue, enumType);
                                return m_IntValue.ToString();
                            }
                        }
                    case InnerType.Invalid:
                        return "INVALID";

                    default:
                        ThrowInvalidCastException(m_Type, typeof(string));
                        return null;
                }
            }
            set
            {
                SetInnerType(InnerType.String);
                m_StringValue = value;
            }
        }

        private EntityScopeData EntityValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.EntityScope:
                        return new EntityScopeData((EntityScopeType) m_NumberValue, m_IntValue, m_StringValue, true).WithLinks(m_StringValue2, m_BoolValue2);

                    case InnerType.Null:
                        return EntityScopeData.Null();

                    default:
                        ThrowInvalidCastException(m_Type, typeof(EntityScopeData));
                        return default(EntityScopeData);
                }
            }
            set
            {
                if (value.Type == EntityScopeType.Null)
                {
                    SetInnerType(InnerType.Null);
                }
                else
                {
                    SetInnerType(InnerType.EntityScope);
                    m_NumberValue = (float) value.Type;
                    switch (value.Type)
                    {
                        case EntityScopeType.ObjectById:
                            m_IntValue = (int) value.IdArg;
                            break;

                        case EntityScopeType.ObjectsWithGroup:
                            m_IntValue = (int) value.GroupArg;
                            break;

                        case EntityScopeType.ObjectsWithName:
                        case EntityScopeType.ObjectsWithPrefab:
                            m_StringValue = value.SearchArg;
                            break;
                    }

                    m_StringValue2 = value.LinksArg;
                    m_BoolValue2 = value.UseFirstLink;
                }
            }
        }

        private RSGroupId GroupIdValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.GroupId:
                        return new RSGroupId(m_IntValue);

                    case InnerType.Null:
                        return RSGroupId.Null;

                    default:
                        ThrowInvalidCastException(m_Type, typeof(RSGroupId));
                        return default(RSGroupId);
                }
            }
            set
            {
                if (value == RSGroupId.Null)
                {
                    SetInnerType(InnerType.Null);
                }
                else
                {
                    SetInnerType(InnerType.GroupId);
                    m_IntValue = (int) value;
                }
            }
        }

        private RSTriggerId TriggerIdValue
        {
            get
            {
                switch (m_Type)
                {
                    case InnerType.TriggerId:
                        return new RSTriggerId(m_IntValue);

                    case InnerType.Null:
                        return RSTriggerId.Null;

                    default:
                        ThrowInvalidCastException(m_Type, typeof(RSTriggerId));
                        return default(RSTriggerId);
                }
            }
            set
            {
                if (value == RSTriggerId.Null)
                {
                    SetInnerType(InnerType.Null);
                }
                else
                {
                    SetInnerType(InnerType.TriggerId);
                    m_IntValue = (int) value;
                }
            }
        }

        #endregion // Value Types

        #region IPreviewable

        public string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (m_Type)
            {
                case InnerType.String:
                    return string.Format("\"{0}\"", m_StringValue);
                case InnerType.Color:
                    return ColorValue.ToString();
                case InnerType.Bool:
                    return BoolValue ? "true" : "false";
                case InnerType.Float:
                    return FloatValue.ToString();
                case InnerType.Int32:
                    return IntValue.ToString();
                case InnerType.Null:
                    return "null";
                case InnerType.Vector2:
                    return AsVector2.ToString();
                case InnerType.Vector3:
                    return AsVector3.ToString();
                case InnerType.Vector4:
                    return AsVector4.ToString();
                case InnerType.EntityScope:
                    return EntityValue.GetPreviewString(inTriggerContext, inLibrary);
                case InnerType.GroupId:
                    return GroupIdValue.ToString(inLibrary);
                case InnerType.TriggerId:
                    return TriggerIdValue.ToString(inLibrary);
                case InnerType.Enum:
                    {
                        Type enumType = Type.GetType(m_StringValue);
                        if (enumType == null)
                        {
                            Debug.LogErrorFormat("Enum type {0} no longer exists", m_StringValue);
                            return m_IntValue.ToString();
                        }
                        
                        try
                        {
                            return Enum.ToObject(enumType, m_IntValue).ToString();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                            Debug.LogErrorFormat("Enum {0} cannot be represented as type {1}", m_IntValue, enumType);
                            return m_IntValue.ToString();
                        }
                    }
                case InnerType.Invalid:
                    return "INVALID";

                default:
                    ThrowInvalidCastException(m_Type, typeof(string));
                    return null;
            }
        }

        #endregion // IPreviewable

        #region Overrides

        public override string ToString()
        {
            return StringValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is RSValue)
                return Equals((RSValue) obj);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = m_Type.GetHashCode();
            hash = (hash << 3) ^ m_IntValue.GetHashCode();
            hash = (hash >> 3) ^ m_NumberValue.GetHashCode();
            hash = (hash << 7) ^ m_VectorValue.GetHashCode();
            hash = (hash >> 1) ^ m_BoolValue2.GetHashCode();
            if (m_StringValue != null)
                hash = (hash >> 5) ^ m_StringValue.GetHashCode();
            if (m_StringValue2 != null)
                hash = (hash << 4) ^ m_StringValue2.GetHashCode();
            return hash;
        }

        static public bool operator ==(RSValue inA, RSValue inB)
        {
            return inA.Equals(inB);
        }

        static public bool operator !=(RSValue inA, RSValue inB)
        {
            return !inA.Equals(inB);
        }

        #endregion // Overrides

        #region IEquatable

        public bool Equals(RSValue inValue)
        {
            return m_Type == inValue.m_Type &&
                m_IntValue == inValue.m_IntValue &&
                m_NumberValue == inValue.m_NumberValue &&
                m_StringValue == inValue.m_StringValue &&
                m_VectorValue == inValue.m_VectorValue &&
                m_StringValue2 == inValue.m_StringValue2 &&
                m_BoolValue2 == inValue.m_BoolValue2;
        }

        #endregion // IEquatable

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 1; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Enum("type", ref m_Type, InnerType.Null, FieldOptions.PreferAttribute);
            switch (m_Type)
            {
                case InnerType.Bool:
                    {
                        bool bValue = ioSerializer.IsWriting ? BoolValue : false;
                        ioSerializer.Serialize("value", ref bValue);
                        if (ioSerializer.IsReading)
                            BoolValue = bValue;
                        break;
                    }

                case InnerType.Color:
                    {
                        Color value = ioSerializer.IsWriting ? ColorValue : default(Color);
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            ColorValue = value;
                        break;
                    }

                case InnerType.Float:
                    {
                        float value = ioSerializer.IsWriting ? FloatValue : 0;
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            FloatValue = value;
                        break;
                    }

                case InnerType.Int32:
                    {
                        int value = ioSerializer.IsWriting ? IntValue : 0;
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            IntValue = value;
                        break;
                    }

                case InnerType.String:
                    {
                        string value = ioSerializer.IsWriting ? StringValue : null;
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            StringValue = value;
                        break;
                    }

                case InnerType.Vector2:
                    {
                        Vector2 value = ioSerializer.IsWriting ? Vector2Value : default(Vector2);
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            Vector2Value = value;
                        break;
                    }

                case InnerType.Vector3:
                    {
                        Vector3 value = ioSerializer.IsWriting ? Vector3Value : default(Vector3);
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            Vector3Value = value;
                        break;
                    }

                case InnerType.Vector4:
                    {
                        Vector4 value = ioSerializer.IsWriting ? Vector4Value : default(Vector4);
                        ioSerializer.Serialize("value", ref value);
                        if (ioSerializer.IsReading)
                            Vector4Value = value;
                        break;
                    }

                case InnerType.Enum:
                    {
                        ioSerializer.Serialize("value", ref m_IntValue);
                        ioSerializer.Serialize("enumType", ref m_StringValue);
                        break;
                    }

                case InnerType.EntityScope:
                    {
                        EntityScopeData value = ioSerializer.IsWriting ? EntityValue : default(EntityScopeData);
                        ioSerializer.Object("value", ref value);
                        if (ioSerializer.IsReading)
                            EntityValue = value;
                        break;
                    }

                case InnerType.GroupId:
                    {
                        RSGroupId value = ioSerializer.IsWriting ? GroupIdValue : default(RSGroupId);
                        ioSerializer.Int32Proxy("value", ref value);
                        if (ioSerializer.IsReading)
                            GroupIdValue = value;
                        break;
                    }

                case InnerType.TriggerId:
                    {
                        RSTriggerId value = ioSerializer.IsWriting ? TriggerIdValue : default(RSTriggerId);
                        ioSerializer.Int32Proxy("value", ref value);
                        if (ioSerializer.IsReading)
                            TriggerIdValue = value;
                        break;
                    }
            }
        }

        #endregion // ISerializedObject

        #region Static

        static public readonly RSValue Null = new RSValue();
        static public readonly RSValue True = new RSValue(true);
        static public readonly RSValue False = new RSValue(false);

        static public readonly RSValue Zero = new RSValue(0);
        static public readonly RSValue One = new RSValue(1);

        static public readonly RSValue Invalid = new RSValue(InnerType.Invalid);

        static public RSValue FromInt(int inValue)
        {
            return new RSValue(inValue);
        }

        static public RSValue FromBool(bool inbValue)
        {
            return inbValue ? True : False;
        }

        static public RSValue FromFloat(float inValue)
        {
            return new RSValue(inValue);
        }

        static public RSValue FromColor(Color inColor)
        {
            return new RSValue(inColor);
        }

        static public RSValue FromString(string inString)
        {
            return new RSValue(inString);
        }

        static public RSValue FromVector4(Vector4 inVector)
        {
            return new RSValue(inVector);
        }

        static public RSValue FromVector3(Vector3 inVector)
        {
            return new RSValue(inVector);
        }

        static public RSValue FromVector2(Vector2 inVector)
        {
            return new RSValue(inVector);
        }

        static public RSValue FromEnum(Enum inValue)
        {
            return new RSValue(inValue);
        }

        static public RSValue FromEntity(EntityScopeData inScope)
        {
            return new RSValue(inScope);
        }

        static public RSValue FromGroupId(RSGroupId inGroupId)
        {
            return new RSValue(inGroupId);
        }

        static public RSValue FromTriggerId(RSTriggerId inGroupId)
        {
            return new RSValue(inGroupId);
        }

        #endregion // Static
    }
}