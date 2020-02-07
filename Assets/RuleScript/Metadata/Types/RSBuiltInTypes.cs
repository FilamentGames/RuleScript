using System;
using RuleScript.Data;
using UnityEngine;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Built-in types.
    /// </summary>
    static public class RSBuiltInTypes
    {
        static internal readonly RSTypeAssembly BaseAssembly;

        static public readonly RSTypeInfo Void = new RSTypeInfo(typeof(void), "Void", RSValue.Null);
        static public readonly RSTypeInfo Any = new RSTypeInfo(typeof(object), "Any", RSValue.Null, TypeFlags.SkipConversionCheck);

        static public readonly RSTypeInfo Int = new RSTypeInfo(typeof(int), "Integer", RSValue.FromInt(0));
        static public readonly RSTypeInfo Float = new RSTypeInfo(typeof(float), "Float", RSValue.FromFloat(0));
        static public readonly RSTypeInfo Bool = new RSTypeInfo(typeof(bool), "Boolean", RSValue.False);
        static public readonly RSTypeInfo Color = new RSTypeInfo(typeof(Color), "Color", RSValue.FromColor(UnityEngine.Color.white));
        static public readonly RSTypeInfo String = new RSTypeInfo(typeof(string), "String", RSValue.FromString(string.Empty));
        static public readonly RSTypeInfo Enum = new RSTypeInfo(typeof(Enum), "Enum", RSValue.Null);

        static public readonly RSTypeInfo Vector2 = new RSTypeInfo(typeof(UnityEngine.Vector2), "Vector2", RSValue.FromVector2(UnityEngine.Vector2.zero));
        static public readonly RSTypeInfo Vector3 = new RSTypeInfo(typeof(UnityEngine.Vector3), "Vector3", RSValue.FromVector3(UnityEngine.Vector3.zero));
        static public readonly RSTypeInfo Vector4 = new RSTypeInfo(typeof(UnityEngine.Vector4), "Vector4", RSValue.FromVector4(UnityEngine.Vector4.zero));

        static public readonly RSTypeInfo Entity = new RSTypeInfo(typeof(IRSEntity), "Entity", RSValue.Null);
        static public readonly RSTypeInfo Component = new RSTypeInfo(typeof(IRSComponent), "Component", RSValue.Null); // TODO(Autumn): Implement
        static public readonly RSTypeInfo GroupId = new RSTypeInfo(typeof(RSGroupId), "Group", RSValue.Null);
        static public readonly RSTypeInfo TriggerId = new RSTypeInfo(typeof(RSTriggerId), "Trigger", RSValue.Null);

        static RSBuiltInTypes()
        {
            BaseAssembly = new RSTypeAssembly();

            BaseAssembly.AddTypeMeta(Void);
            BaseAssembly.AddTypeMeta(Any);
            BaseAssembly.AddTypeMeta(Int);
            BaseAssembly.AddTypeMeta(Float);
            BaseAssembly.AddTypeMeta(Bool);
            BaseAssembly.AddTypeMeta(Color);
            BaseAssembly.AddTypeMeta(String);
            BaseAssembly.AddTypeMeta(Enum);
            BaseAssembly.AddTypeMeta(Vector2);
            BaseAssembly.AddTypeMeta(Vector3);
            BaseAssembly.AddTypeMeta(Vector4);
            BaseAssembly.AddTypeMeta(Entity);
            BaseAssembly.AddTypeMeta(Component);
            BaseAssembly.AddTypeMeta(GroupId);
            BaseAssembly.AddTypeMeta(TriggerId);

            // CONVERSIONS

            Int.AddConversions(Float);
            Enum.AddConversions(Int);

            // COMPARISONS

            Any.AllowNumericComparisons();
            Any.AllowStringComparisons();
            Any.AllowBooleanComparisons();

            Int.AllowNumericComparisons();
            Float.AllowNumericComparisons();

            Bool.AllowEqualityComparisons();
            Bool.AllowBooleanComparisons();
            Color.AllowEqualityComparisons();
            String.AllowEqualityComparisons();
            String.AllowStringComparisons();

            Vector2.AllowEqualityComparisons();
            Vector3.AllowEqualityComparisons();
            Vector4.AllowEqualityComparisons();

            Entity.AllowEqualityComparisons();
            GroupId.AllowEqualityComparisons();
            TriggerId.AllowEqualityComparisons();
        }
    }
}