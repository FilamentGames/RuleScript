using System;
using BeauData;
using RuleScript.Metadata;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Data
{
    [Serializable]
    public struct NestedValue : ISerializedObject, ISerializedVersion, IRSPreviewable, IEquatable<NestedValue>
    {
        #region Inspector

        [SerializeField] private ResolvableValueMode m_Mode;
        [SerializeField] private RSValue m_Value;
        [SerializeField] private EntityScopedIdentifier m_Query;
        [SerializeField] private RegisterIndex m_Register;

        #endregion // Inspector

        public ResolvableValueMode Mode { get { return m_Mode; } }

        public RSValue Value
        {
            get
            {
                Assert.True(m_Mode == ResolvableValueMode.Value, "Value not available for mode {0}", m_Mode);
                return m_Value;
            }
        }

        public EntityScopedIdentifier Query
        {
            get
            {
                Assert.True(m_Mode == ResolvableValueMode.Query, "Query not available for mode {0}", m_Mode);
                return m_Query;
            }
        }

        public RegisterIndex Register
        {
            get
            {
                Assert.True(m_Mode == ResolvableValueMode.Register, "Register is not available for mode {0}", m_Mode);
                return m_Register;
            }
        }

        public RSTypeInfo TypeInfo(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (m_Mode)
            {
                case ResolvableValueMode.Value:
                    return RSInterop.RSTypeFor(m_Value, inLibrary.TypeAssembly);
                case ResolvableValueMode.Argument:
                    return inTriggerContext?.ParameterType?.Type;
                case ResolvableValueMode.Query:
                    return inLibrary.GetQuery(m_Query.Id)?.ReturnType;
                case ResolvableValueMode.Register:
                    return RSBuiltInTypes.Any;

                default:
                    throw new InvalidOperationException("Unknown ResolvableValueMode " + m_Mode);
            }
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 2; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Enum("mode", ref m_Mode, FieldOptions.PreferAttribute);

            switch (m_Mode)
            {
                case ResolvableValueMode.Value:
                    ioSerializer.Object("value", ref m_Value);
                    break;

                case ResolvableValueMode.Query:
                    ioSerializer.Object("query", ref m_Query);
                    break;
            }
        }

        #endregion // ISerializedObject

        #region IPreviewable

        public string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (m_Mode)
            {
                case ResolvableValueMode.Value:
                    return m_Value.GetPreviewString(inTriggerContext, inLibrary);

                case ResolvableValueMode.Argument:
                    return string.Format("Arg({0})", inTriggerContext?.ParameterType?.Name ?? "TriggerArg");

                case ResolvableValueMode.Query:
                    return m_Query.GetPreviewStringAsQuery(inTriggerContext, inLibrary);

                case ResolvableValueMode.Register:
                    return string.Format("Register({0})", m_Register);

                default:
                    return "[No Value]";
            }
        }

        #endregion // IPreviewable

        #region IEquatable

        public bool Equals(NestedValue other)
        {
            return m_Mode == other.m_Mode &&
                m_Value == other.m_Value &&
                m_Query == other.m_Query &&
                m_Register == other.m_Register;
        }

        #endregion // IEquatable

        #region Overrides

        public override bool Equals(object obj)
        {
            if (obj is NestedValue)
                return Equals((NestedValue) obj);
            return false;
        }

        public override int GetHashCode()
        {
            int hash = m_Mode.GetHashCode();
            hash = (hash >> 3) ^ m_Value.GetHashCode();
            return hash;
        }

        static public bool operator ==(NestedValue a, NestedValue b)
        {
            return a.Equals(b);
        }

        static public bool operator !=(NestedValue a, NestedValue b)
        {
            return !a.Equals(b);
        }

        #endregion // Overrides

        #region Static

        static public implicit operator NestedValue(RSValue inValue)
        {
            return FromValue(inValue);
        }

        static public NestedValue FromValue(RSValue inValue)
        {
            return new NestedValue()
            {
                m_Mode = ResolvableValueMode.Value,
                    m_Value = inValue
            };
        }

        static public NestedValue FromArgument()
        {
            return new NestedValue()
            {
                m_Mode = ResolvableValueMode.Argument
            };
        }

        static public NestedValue FromQuery(EntityScopedIdentifier inQuery)
        {
            return new NestedValue()
            {
                m_Mode = ResolvableValueMode.Query,
                m_Query = inQuery
            };
        }

        static public NestedValue FromRegister(RegisterIndex inRegister)
        {
            return new NestedValue()
            {
                m_Mode = ResolvableValueMode.Register,
                    m_Register = inRegister
            };
        }

        #endregion // Static
    }
}