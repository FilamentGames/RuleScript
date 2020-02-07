/*
 * Copyright (C) 2019 - 2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Sept 2019
 * 
 * File:    RSResolvableValueData.cs
 * Purpose: Serializable data about a resolvable value.
 */

using System;
using BeauData;
using BeauUtil;
using RuleScript.Metadata;
using RuleScript.Runtime;

namespace RuleScript.Data
{
    [Serializable]
    public class RSResolvableValueData : ISerializedObject, ISerializedVersion, IRSPreviewable, ICopyCloneable<RSResolvableValueData>
    {
        #region Inspector

        public ResolvableValueMode Mode;

        public RSValue Value;
        public EntityScopedIdentifier Query;
        public NestedValue[] QueryArguments;
        public RegisterIndex Register = RegisterIndex.Invalid;

        #endregion // Inspector

        public bool IsMultiValue()
        {
            if (Mode == ResolvableValueMode.Query)
                return Query.Scope.IsMultiTarget();

            return false;
        }

        public RSTypeInfo TypeInfo(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (Mode)
            {
                case ResolvableValueMode.Value:
                    return RSInterop.RSTypeFor(Value, inLibrary.TypeAssembly);
                case ResolvableValueMode.Query:
                    return inLibrary.GetQuery(Query.Id)?.ReturnType;
                case ResolvableValueMode.Argument:
                    return inTriggerContext?.ParameterType?.Type;
                case ResolvableValueMode.Register:
                    return RSBuiltInTypes.Any;

                default:
                    throw new InvalidOperationException("Unknown ResolvableValueMode " + Mode);
            }
        }

        #region ISerializedObject

        ushort ISerializedVersion.Version { get { return 2; } }

        void ISerializedObject.Serialize(Serializer ioSerializer)
        {
            ioSerializer.Enum("mode", ref Mode, FieldOptions.PreferAttribute);

            switch (Mode)
            {
                case ResolvableValueMode.Value:
                    ioSerializer.Object("value", ref Value);
                    break;

                case ResolvableValueMode.Query:
                    ioSerializer.Object("query", ref Query);
                    ioSerializer.ObjectArray("arguments", ref QueryArguments, FieldOptions.Optional);
                    break;

                case ResolvableValueMode.Register:
                    ioSerializer.Enum("register", ref Register, RegisterIndex.Invalid, FieldOptions.Optional);
                    break;
            }
        }

        #endregion // ISerializedObject

        #region IPreviewable

        public string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary)
        {
            switch (Mode)
            {
                case ResolvableValueMode.Value:
                    return Value.GetPreviewString(inTriggerContext, inLibrary);

                case ResolvableValueMode.Argument:
                    return string.Format("Arg({0})", inTriggerContext?.ParameterType?.Name ?? "TriggerArg");

                case ResolvableValueMode.Query:
                    {
                        using(PooledStringBuilder psb = PooledStringBuilder.Alloc())
                        {
                            var sb = psb.Builder;

                            sb.Append(Query.GetPreviewStringAsQuery(inTriggerContext, inLibrary));

                            if (QueryArguments != null && QueryArguments.Length > 0)
                            {
                                sb.Append("(");

                                RSQueryInfo queryInfo = inLibrary.GetQuery(Query.Id);
                                for (int i = 0; i < QueryArguments.Length; ++i)
                                {
                                    if (i > 0)
                                        sb.Append("; ");

                                    if (queryInfo != null && i < queryInfo.Parameters.Length)
                                        sb.Append(queryInfo.Parameters[i].Name);
                                    else
                                        sb.Append(i);

                                    sb.Append(": ");

                                    sb.Append(QueryArguments[i].GetPreviewString(inTriggerContext, inLibrary));
                                }

                                sb.Append(")");
                            }

                            return sb.ToString();
                        }
                    }

                case ResolvableValueMode.Register:
                    return string.Format("Register({0})", Register);

                default:
                    return "[No Value]";
            }
        }

        #endregion // IPreviewable

        #region ICloneable

        public RSResolvableValueData Clone()
        {
            RSResolvableValueData clone = new RSResolvableValueData();
            clone.Mode = Mode;
            clone.Value = Value;
            clone.Query = Query;
            clone.QueryArguments = CloneUtils.Clone(QueryArguments);
            clone.Register = Register;
            return clone;
        }

        public void CopyFrom(RSResolvableValueData inValue)
        {
            Mode = inValue.Mode;
            Value = inValue.Value;
            Query = inValue.Query;
            QueryArguments = CloneUtils.Clone(inValue.QueryArguments);
            Register = inValue.Register;
        }

        #endregion // ICloneable

        static public void SetAsValue(ref RSResolvableValueData ioValue, RSValue inValue)
        {
            if (ioValue == null)
                ioValue = new RSResolvableValueData();

            ioValue.Mode = ResolvableValueMode.Value;
            ioValue.Value = inValue;
            ioValue.Query = default(EntityScopedIdentifier);
            ioValue.QueryArguments = null;
            ioValue.Register = RegisterIndex.Invalid;
        }

        static public void SetAsArgument(ref RSResolvableValueData ioValue)
        {
            if (ioValue == null)
                ioValue = new RSResolvableValueData();

            ioValue.Mode = ResolvableValueMode.Argument;
            ioValue.Value = RSValue.Null;
            ioValue.Query = default(EntityScopedIdentifier);
            ioValue.QueryArguments = null;
            ioValue.Register = RegisterIndex.Invalid;
        }

        static public void SetAsQuery(ref RSResolvableValueData ioValue, EntityScopedIdentifier inQuery)
        {
            if (ioValue == null)
                ioValue = new RSResolvableValueData();

            ioValue.Mode = ResolvableValueMode.Query;
            ioValue.Value = RSValue.Null;
            ioValue.Query = inQuery;
            ioValue.QueryArguments = null;
            ioValue.Register = RegisterIndex.Invalid;
        }

        static public void SetAsRegister(ref RSResolvableValueData ioValue, RegisterIndex inRegister)
        {
            if (ioValue == null)
                ioValue = new RSResolvableValueData();

            ioValue.Mode = ResolvableValueMode.Register;
            ioValue.Value = RSValue.Null;
            ioValue.Query = default(EntityScopedIdentifier);
            ioValue.QueryArguments = null;
            ioValue.Register = inRegister;
        }
    }
}