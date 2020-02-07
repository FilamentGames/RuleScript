using System;
using RuleScript.Data;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Runtime
{
    /// <summary>
    /// Interop between C# and RS values.
    /// </summary>
    static internal class RSInterop
    {
        /// <summary>
        /// Converts an RSValue to the given C# type.
        /// </summary>
        static public T ToObject<T>(RSValue inValue, ExecutionScope inContext)
        {
            return (T) ToObject(typeof(T), inValue, inContext);
        }

        /// <summary>
        /// Converts an RSValue to its closest mapped C# type.
        /// </summary>
        static public object ToObject(RSValue inValue, ExecutionScope inContext)
        {
            return ToObject(inValue.GetInteropType(), inValue, inContext);
        }

        /// <summary>
        /// Converts an RSValue to the given C# type.
        /// </summary>
        static public object ToObject(Type inType, RSValue inValue, ExecutionScope inContext)
        {
            if (inType == null || inType == typeof(void))
                return null;

            if (inType == typeof(object))
                return inValue;

            if (inType == typeof(RSValue))
                return inValue;

            if (inType.IsEnum)
            {
                return inValue.AsEnum(inType);
            }

            switch (Type.GetTypeCode(inType))
            {
                case TypeCode.Boolean:
                    return inValue.AsBool;
                case TypeCode.Byte:
                    return (byte) inValue.AsInt;
                case TypeCode.Char:
                    return (char) inValue.AsInt;
                case TypeCode.Double:
                    return (double) inValue.AsFloat;
                case TypeCode.Int16:
                    return (Int16) inValue.AsInt;
                case TypeCode.Int32:
                    return inValue.AsInt;
                case TypeCode.Int64:
                    return (Int64) inValue.AsInt;
                case TypeCode.SByte:
                    return (sbyte) inValue.AsInt;
                case TypeCode.Single:
                    return inValue.AsFloat;
                case TypeCode.String:
                    return inValue.AsString;
                case TypeCode.UInt16:
                    return (UInt16) inValue.AsInt;
                case TypeCode.UInt32:
                    return (UInt32) inValue.AsInt;
                case TypeCode.UInt64:
                    return (UInt64) inValue.AsInt;

                case TypeCode.Object:
                    {
                        if (inType == typeof(Color))
                        {
                            return inValue.AsColor;;
                        }
                        if (inType == typeof(Vector2))
                        {
                            return inValue.AsVector2;
                        }
                        if (inType == typeof(Vector3))
                        {
                            return inValue.AsVector3;
                        }
                        if (inType == typeof(Vector4))
                        {
                            return inValue.AsVector4;
                        }
                        if (inType == typeof(RSGroupId))
                        {
                            return inValue.AsGroupId;
                        }
                        if (inType == typeof(RSTriggerId))
                        {
                            return inValue.AsTriggerId;
                        }
                        if (typeof(IRSEntity).IsAssignableFrom(inType))
                        {
                            return inContext.ResolveEntity(inValue.AsEntity).ForceSingle();
                        }
                        break;
                    }
            }

            throw new ArgumentException(string.Format("Unable to convert RSValue {0} to object of type {1}", inValue, inType), "inValue");
        }

        /// <summary>
        /// Converts a C# object to an RSValue.
        /// </summary>
        static public RSValue ToRSValue(object inObject)
        {
            if (inObject == null)
                return RSValue.Null;

            Type objType = inObject.GetType();

            if (objType == typeof(RSValue))
                return (RSValue) inObject;

            if (objType.IsEnum)
            {
                return RSValue.FromEnum((Enum) inObject);
            }

            switch (Type.GetTypeCode(objType))
            {
                case TypeCode.Boolean:
                    return RSValue.FromBool((bool) inObject);
                case TypeCode.Byte:
                    return RSValue.FromInt((byte) inObject);
                case TypeCode.Char:
                    return RSValue.FromInt((char) inObject);
                case TypeCode.Double:
                    Log.Warn("[RSInterop] Truncation from Double to Single");
                    return RSValue.FromFloat((float) (double) inObject);
                case TypeCode.Int16:
                    return RSValue.FromInt((Int16) inObject);
                case TypeCode.Int32:
                    return RSValue.FromInt((Int32) inObject);
                case TypeCode.SByte:
                    return RSValue.FromInt((sbyte) inObject);
                case TypeCode.Single:
                    return RSValue.FromFloat((float) inObject);
                case TypeCode.String:
                    return RSValue.FromString((string) inObject);
                case TypeCode.UInt16:
                    return RSValue.FromInt((UInt16) inObject);
                case TypeCode.UInt32:
                    Log.Warn("[RSInterop] Truncation from UInt32 to Int32");
                    return RSValue.FromInt((int) (UInt32) inObject);
                case TypeCode.UInt64:
                    Log.Warn("[RSInterop] Truncation from UInt64 to Int32");
                    return RSValue.FromInt((int) (UInt64) inObject);

                case TypeCode.Object:
                    {
                        if (objType == typeof(Color))
                        {
                            return RSValue.FromColor((Color) inObject);
                        }
                        if (objType == typeof(Vector2))
                        {
                            return RSValue.FromVector2((Vector2) inObject);
                        }
                        if (objType == typeof(Vector3))
                        {
                            return RSValue.FromVector3((Vector3) inObject);
                        }
                        if (objType == typeof(Vector4))
                        {
                            return RSValue.FromVector4((Vector4) inObject);
                        }
                        if (objType == typeof(RSGroupId))
                        {
                            return RSValue.FromGroupId((RSGroupId) inObject);
                        }
                        if (objType == typeof(RSTriggerId))
                        {
                            return RSValue.FromTriggerId((RSTriggerId) inObject);
                        }
                        if (typeof(IRSEntity).IsAssignableFrom(objType))
                        {
                            IRSEntity entity = (IRSEntity) inObject;
                            EntityScopeData scope = EntityScopeData.Entity(entity.Id);
                            return RSValue.FromEntity(scope);
                        }
                        break;
                    }
            }

            throw new ArgumentException(string.Format("Unable to convert object of type {0} to RSValue", objType.Name), "inObject");
        }

        /// <summary>
        /// Retrieves the type metadata for the given C# type.
        /// </summary>
        static public RSTypeInfo RSTypeFor(Type inType, RSTypeAssembly inAssembly)
        {
            if (inType == null || inType == typeof(void))
                return RSBuiltInTypes.Void;

            if (inType == typeof(object) || inType == typeof(RSValue))
                return RSBuiltInTypes.Any;

            if (inType.IsEnum)
            {
                return inAssembly.GetTypeMeta(inType);
            }

            switch (Type.GetTypeCode(inType))
            {
                case TypeCode.Boolean:
                    return RSBuiltInTypes.Bool;
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return RSBuiltInTypes.Int;
                case TypeCode.Double:
                case TypeCode.Single:
                    return RSBuiltInTypes.Float;
                case TypeCode.String:
                    return RSBuiltInTypes.String;
                case TypeCode.Object:
                    {
                        if (inType == typeof(Color))
                        {
                            return RSBuiltInTypes.Color;
                        }
                        if (inType == typeof(Vector2))
                        {
                            return RSBuiltInTypes.Vector2;
                        }
                        if (inType == typeof(Vector3))
                        {
                            return RSBuiltInTypes.Vector3;
                        }
                        if (inType == typeof(Vector4))
                        {
                            return RSBuiltInTypes.Vector4;
                        }
                        if (inType == typeof(RSGroupId))
                        {
                            return RSBuiltInTypes.GroupId;
                        }
                        if (inType == typeof(RSTriggerId))
                        {
                            return RSBuiltInTypes.TriggerId;
                        }
                        if (typeof(IRSEntity).IsAssignableFrom(inType))
                        {
                            return RSBuiltInTypes.Entity;
                        }

                        break;
                    }
            }

            throw new ArgumentException(string.Format("Unable to find RSType for type {0}", inType.Name), "inType");
        }

        /// <summary>
        /// Retrieves the type metadata for the given value.
        /// </summary>
        static public RSTypeInfo RSTypeFor(RSValue inValue, RSTypeAssembly inAssembly)
        {
            return RSTypeFor(inValue.GetInteropType(), inAssembly);
        }
    }
}