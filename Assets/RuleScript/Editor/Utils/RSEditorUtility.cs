using System;
using System.Collections.Generic;
using BeauData;
using BeauUtil.Editor;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using RuleScript.Validation;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    static public class RSEditorUtility
    {
        static RSEditorUtility()
        {
            InitializeLists();
        }

        static private Dictionary<Type, RSLibrary> s_Libraries = new Dictionary<Type, RSLibrary>();
        static private IRSEditorPlugin s_Plugin = StubEditorPlugin.Instance;

        /// <summary>
        /// Default library.
        /// </summary>
        /// <returns></returns>
        static public RSLibrary DefaultLibrary()
        {
            return DefaultLibrary(typeof(IRSRuntimeEntity));
        }

        /// <summary>
        /// Default library for the given entity type.
        /// </summary>
        static public RSLibrary DefaultLibrary(Type inEntityType)
        {
            RSLibrary library;
            if (!s_Libraries.TryGetValue(inEntityType, out library))
            {
                library = new RSLibrary(inEntityType);
                Action<float, string> progressUpdate = (f, s) =>
                {
                    EditorUtility.DisplayProgressBar("Loading RuleScript library for " + inEntityType.Name, s, f);
                };

                try
                {
                    System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    library.ScanAssemblies(progressUpdate);
                    stopwatch.Stop();
                    Debug.Log("[RSEditorUtility] Loaded default library for " + inEntityType.Name + " in " + stopwatch.Elapsed.TotalSeconds + " seconds");
                }
                finally
                {
                    s_Libraries.Add(inEntityType, library);
                    EditorUtility.ClearProgressBar();
                }
            }
            return library;
        }

        /// <summary>
        /// Editor plugin.
        /// </summary>
        static public IRSEditorPlugin EditorPlugin
        {
            get { return s_Plugin; }
            set
            {
                if (ReferenceEquals(value, null))
                    value = StubEditorPlugin.Instance;

                if (s_Plugin == value)
                    return;

                s_Plugin = value;
            }
        }

        #region Lists

        static internal readonly RSElementList<RSTriggerInfo> s_TriggerElements = new RSElementList<RSTriggerInfo>("[no trigger]");
        static internal readonly RSElementList<RSActionInfo> s_ActionElements = new RSElementList<RSActionInfo>("[no action]");
        static internal readonly RSElementList<RSQueryInfo> s_QueryElements = new RSElementList<RSQueryInfo>("[no query]");
        static internal readonly RSElementList<RSGroupInfo> s_GroupElements = new RSElementList<RSGroupInfo>("[no group]");

        static internal readonly NamedItemList<EntityScopeType> s_EntityScopeTypes = new NamedItemList<EntityScopeType>();
        static internal readonly NamedItemList<ResolvableValueMode> s_ResolvableValueModes = new NamedItemList<ResolvableValueMode>();
        static internal readonly NamedItemList<ResolvableValueMode> s_ResolvableValueModesNoRegister = new NamedItemList<ResolvableValueMode>();
        static internal readonly NamedItemList<ResolvableValueMode> s_ResolvableValueModesNoValue = new NamedItemList<ResolvableValueMode>();
        static internal readonly NamedItemList<ResolvableValueMode> s_ResolvableValueModesNoValueOrRegister = new NamedItemList<ResolvableValueMode>();
        static internal readonly NamedItemList<CompareOperator> s_ComparisonOperators = new NamedItemList<CompareOperator>();
        static internal readonly NamedItemList<Serializer.Format> s_SerializeFormats = new NamedItemList<Serializer.Format>();

        static internal NamedItemList<ResolvableValueMode> GetResolvableValueModes(RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            bool bDisallowDirectValue = (inExpectedType == null || inExpectedType == RSBuiltInTypes.Any || inFlags.Has(RSValidationFlags.DisallowDirectValue));
            bool bDisallowRegister = inFlags.Has(RSValidationFlags.DisallowRegisters);

            if (bDisallowDirectValue)
            {
                if (bDisallowDirectValue)
                    return s_ResolvableValueModesNoValueOrRegister;
                return s_ResolvableValueModesNoRegister;
            }
            if (bDisallowRegister)
            {
                return s_ResolvableValueModesNoRegister;
            }
            return s_ResolvableValueModes;
        }

        static private void InitializeLists()
        {
            s_EntityScopeTypes.Add(EntityScopeType.Null, "[no entity]", -1);
            s_EntityScopeTypes.Add(EntityScopeType.Self, "Self", 0);
            s_EntityScopeTypes.Add(EntityScopeType.Global, "Global", 1);
            s_EntityScopeTypes.Add(EntityScopeType.ObjectById, "Entity", 2);
            s_EntityScopeTypes.Add(EntityScopeType.ObjectsWithName, "Search/By Name", 3);
            s_EntityScopeTypes.Add(EntityScopeType.ObjectsWithPrefab, "Search/By Prefab", 4);
            s_EntityScopeTypes.Add(EntityScopeType.ObjectsWithGroup, "Search/By Group", 5);
            s_EntityScopeTypes.Add(EntityScopeType.Argument, "Trigger Parameter", 6);
            s_EntityScopeTypes.Add(EntityScopeType.ObjectInRegister, "Local Var", 7);

            s_ResolvableValueModes.Add(ResolvableValueMode.Value, "Value", 0);
            s_ResolvableValueModes.Add(ResolvableValueMode.Query, "Entity Query", 1);
            s_ResolvableValueModes.Add(ResolvableValueMode.Argument, "Trigger Parameter", 2);
            s_ResolvableValueModes.Add(ResolvableValueMode.Register, "Local Var", 3);

            s_ResolvableValueModesNoRegister.Add(ResolvableValueMode.Value, "Value", 0);
            s_ResolvableValueModesNoRegister.Add(ResolvableValueMode.Query, "Entity Query", 1);
            s_ResolvableValueModesNoRegister.Add(ResolvableValueMode.Argument, "Trigger Parameter", 2);

            s_ResolvableValueModesNoValue.Add(ResolvableValueMode.Query, "Entity Query", 0);
            s_ResolvableValueModesNoValue.Add(ResolvableValueMode.Argument, "Trigger Parameter", 1);
            s_ResolvableValueModesNoValue.Add(ResolvableValueMode.Register, "Local Var", 2);

            s_ResolvableValueModesNoValueOrRegister.Add(ResolvableValueMode.Query, "Entity Query", 0);
            s_ResolvableValueModesNoValueOrRegister.Add(ResolvableValueMode.Argument, "Trigger Parameter", 1);

            s_SerializeFormats.Add(Serializer.Format.JSON, "JSON", 0);
            s_SerializeFormats.Add(Serializer.Format.XML, "XML", 1);
            s_SerializeFormats.Add(Serializer.Format.Binary, "Binary", 2);
            s_SerializeFormats.Add(Serializer.Format.GZIP, "GZIP", 3);
        }

        #endregion // Lists
    }
}