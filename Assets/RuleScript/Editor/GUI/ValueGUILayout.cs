using System;
using System.Collections;
using System.Collections.Generic;
using BeauUtil.Editor;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Validation;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    /// <summary>
    /// GUI editors for basic value types.
    /// </summary>
    static public class ValueGUILayout
    {
        #region Content

        static public class Content
        {
            // Entity Scope
            static public readonly GUIContent EntityScopeEntityIdLabel = new GUIContent("Entity", "Specify an exact entity");
            static public readonly GUIContent EntityScopeRegisterLabel = new GUIContent("Variable Slot", "Which variable slot contains the target entity");
            static public readonly GUIContent EntityScopeGroupLabel = new GUIContent("Group", "All entities with the given group");
            static public readonly GUIContent EntityScopeNameLabel = new GUIContent("Name", "All entities that match the given name");
            static public readonly GUIContent EntityScopePrefabLabel = new GUIContent("Prefab", "All entities that match the given prefab name filter");
            static public readonly GUIContent EntityScopeUseFirstLabel = new GUIContent("Use First?", "If unchecked, all entities that match these conditions will be returned.");
            static public readonly GUIContent EntityScopeLinksLabel = new GUIContent("Links", "Entities with links from the previously retrieved entities");
            static public readonly GUIContent EntityScopeUseFirstLinkLabel = new GUIContent("Use First?", "If unchecked, all entities with the given link id will be returned.");

            // Query
            static public readonly GUIContent QueryIdLabel = new GUIContent("Query Id", "Which query to perform on the target");
            
            // Action
            static public readonly GUIContent ActionIdLabel = new GUIContent("Action Id", "Which action to perform on the target");
        }

        #endregion // Content

        #region Entity Scope

        /// <summary>
        /// Renders a layout editor for an EntityScopeData.
        /// </summary>
        static public EntityScopeData EntityScopeField(GUIContent inLabel, EntityScopeData inScope, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoEntityScopeField(inLabel, inScope, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for an EntityScopeData.
        /// </summary>
        static public EntityScopeData EntityScopeField(EntityScopeData inScope, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoEntityScopeField(GUIContent.none, inScope, inFlags, inContext);
            }
        }

        static private EntityScopeData DoEntityScopeField(GUIContent inLabel, EntityScopeData inScope, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            bool bForceFirst = inFlags.Has(RSValidationFlags.RequireSingleEntity);

            EditorGUILayout.BeginVertical();

            EntityScopeType currentType = inScope.Type;
            EntityScopeType nextType = ListGUILayout.Popup(inLabel, inScope.Type, RSEditorUtility.s_EntityScopeTypes);

            RSEntityId entityId = RSEntityId.Null;
            RSGroupId groupId = RSGroupId.Null;
            string search = string.Empty;
            bool useFirst = bForceFirst;
            string links = string.Empty;
            bool useFirstLinks = bForceFirst;
            RegisterIndex register = RegisterIndex.Register0;

            using(new EditorGUI.IndentLevelScope())
            {
                switch (currentType)
                {
                    case EntityScopeType.ObjectById:
                        {
                            entityId = RSEditorUtility.EditorPlugin.EntityIdGUIField(Content.EntityScopeEntityIdLabel, inScope.IdArg, inContext.Manager);
                            if (entityId == RSEntityId.Null && !inFlags.Has(RSValidationFlags.AllowNullEntity))
                            {
                                EditorGUILayout.HelpBox("Null entity not allowed in this context", MessageType.Error);
                            }
                            break;
                        }

                    case EntityScopeType.ObjectInRegister:
                        {
                            if (inFlags.Has(RSValidationFlags.DisallowRegisters))
                            {
                                EditorGUILayout.HelpBox("Local vars not allowed in this context", MessageType.Error);
                            }
                            else
                            {
                                register = inScope.RegisterArg;
                                register = (RegisterIndex) EnumGUILayout.EnumField(Content.EntityScopeRegisterLabel, register);
                            }
                            break;
                        }

                    case EntityScopeType.ObjectsWithGroup:
                        {
                            groupId = inScope.GroupArg;
                            useFirst = bForceFirst || inScope.UseFirst;

                            using(new EditorGUILayout.HorizontalScope())
                            {
                                groupId = LibraryGUILayout.GroupSelector(Content.EntityScopeGroupLabel, groupId, inContext.Library);
                                using(new EditorGUI.DisabledScope(bForceFirst))
                                using(new RSGUI.LabelWidthScope(100))
                                {
                                    useFirst = EditorGUILayout.Toggle(Content.EntityScopeGroupLabel, useFirst, GUILayout.Width(120));
                                }
                            }
                            break;
                        }

                    case EntityScopeType.ObjectsWithName:
                    case EntityScopeType.ObjectsWithPrefab:
                        {
                            search = inScope.SearchArg;
                            useFirst = bForceFirst || inScope.UseFirst;

                            using(new EditorGUILayout.HorizontalScope())
                            {
                                search = EditorGUILayout.TextField(currentType == EntityScopeType.ObjectsWithName ? Content.EntityScopeNameLabel : Content.EntityScopePrefabLabel, search);
                                using(new EditorGUI.DisabledScope(bForceFirst))
                                using(new RSGUI.LabelWidthScope(100))
                                {
                                    useFirst = EditorGUILayout.Toggle(Content.EntityScopeUseFirstLabel, useFirst, GUILayout.Width(120));
                                }
                            }
                            break;
                        }

                    case EntityScopeType.Null:
                        {
                            if (!inFlags.Has(RSValidationFlags.AllowNullEntity))
                            {
                                EditorGUILayout.HelpBox("Null entity not allowed in this context", MessageType.Error);
                            }
                            break;
                        }

                    case EntityScopeType.Invalid:
                        {
                            EditorGUILayout.HelpBox("Missing entity not allowed", MessageType.Error);
                            break;
                        }

                    case EntityScopeType.Global:
                        {
                            if (!inFlags.Has(RSValidationFlags.AllowGlobalEntity))
                            {
                                EditorGUILayout.HelpBox("Global entity not allowed in this context", MessageType.Error);
                            }
                            break;
                        }

                    case EntityScopeType.Argument:
                        {
                            if (inContext.Trigger == null)
                            {
                                EditorGUILayout.HelpBox("No argument available: No Trigger", MessageType.Error);
                            }
                            else if (inContext.Trigger.ParameterType == null)
                            {
                                EditorGUILayout.HelpBox(string.Format("No argument available: Trigger {0} has no argument", inContext.Trigger.Name), MessageType.Error);
                            }
                            else if (!inContext.Trigger.ParameterType.Type.CanConvert(RSBuiltInTypes.Entity))
                            {
                                EditorGUILayout.HelpBox(string.Format("No argument available: Trigger {0} has incompatible argument type {1}, which cannot convert to an Entity", inContext.Trigger.Name, inContext.Trigger.ParameterType.Type), MessageType.Error);
                            }
                            break;
                        }
                }

                if (inScope.SupportsLinks())
                {
                    links = inScope.LinksArg;
                    useFirstLinks = bForceFirst || inScope.UseFirstLink;

                    using(new EditorGUILayout.HorizontalScope())
                    {
                        links = EditorGUILayout.TextField(Content.EntityScopeLinksLabel, links);
                        using(new EditorGUI.DisabledScope(bForceFirst))
                        using(new RSGUI.LabelWidthScope(100))
                        {
                            useFirstLinks = EditorGUILayout.Toggle(Content.EntityScopeUseFirstLinkLabel, useFirstLinks, GUILayout.Width(120));
                        }
                    }
                }
            }

            EditorGUILayout.EndVertical();

            switch (nextType)
            {
                case EntityScopeType.Self:
                    return EntityScopeData.Self().WithLinks(links, useFirstLinks);
                case EntityScopeType.Argument:
                    return EntityScopeData.Argument().WithLinks(links, useFirstLinks);
                case EntityScopeType.Global:
                    return EntityScopeData.Global();

                case EntityScopeType.ObjectById:
                    return EntityScopeData.Entity(entityId).WithLinks(links, useFirstLinks);
                case EntityScopeType.ObjectInRegister:
                    return EntityScopeData.Register(register).WithLinks(links, useFirstLinks);

                case EntityScopeType.ObjectsWithGroup:
                    return EntityScopeData.WithGroup(groupId, useFirst).WithLinks(links, useFirstLinks);
                case EntityScopeType.ObjectsWithName:
                    return EntityScopeData.WithName(search, useFirst).WithLinks(links, useFirstLinks);
                case EntityScopeType.ObjectsWithPrefab:
                    return EntityScopeData.WithPrefab(search, useFirst).WithLinks(links, useFirstLinks);

                case EntityScopeType.Invalid:
                    return EntityScopeData.Invalid();

                case EntityScopeType.Null:
                default:
                    return EntityScopeData.Null();
            }
        }

        #endregion // Entity Scope

        #region RSValue

        /// <summary>
        /// Renders a layout editor for an RSValue.
        /// </summary>
        static public RSValue RSValueField(GUIContent inLabel, RSValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoRSValueField(inLabel, inValue, inExpectedType, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for an RSValue.
        /// </summary>
        static public RSValue RSValueField(RSValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoRSValueField(GUIContent.none, inValue, inExpectedType, inFlags, inContext);
            }
        }

        static private RSValue DoRSValueField(GUIContent inLabel, RSValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            Type systemType = inExpectedType.SystemType;
            if (systemType.IsEnum)
            {
                Enum currentValue;
                try
                {
                    currentValue = inValue.AsEnum();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    currentValue = inExpectedType.DefaultValue.AsEnum();
                }
                Enum nextValue = EnumGUILayout.EnumField(inLabel, currentValue);
                return RSValue.FromEnum(nextValue);
            }

            if (inExpectedType == RSBuiltInTypes.Int)
            {
                int currentValue = inValue.AsInt;
                int nextValue = EditorGUILayout.DelayedIntField(inLabel, currentValue);
                return RSValue.FromInt(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Float)
            {
                float currentValue = inValue.AsFloat;
                float nextValue = EditorGUILayout.DelayedFloatField(inLabel, currentValue);
                return RSValue.FromFloat(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Bool)
            {
                bool currentValue = inValue.AsBool;
                bool nextValue = EditorGUILayout.Toggle(inLabel, currentValue);
                return RSValue.FromBool(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Color)
            {
                Color currentValue = inValue.AsColor;
                Color nextValue = EditorGUILayout.ColorField(inLabel, currentValue);
                return RSValue.FromColor(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.String)
            {
                string currentValue = inValue.AsString;
                string nextValue = EditorGUILayout.TextField(inLabel, currentValue);
                return RSValue.FromString(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Vector2)
            {
                Vector2 currentValue = inValue.AsVector2;
                Vector2 nextValue = EditorGUILayout.Vector2Field(inLabel, currentValue);
                return RSValue.FromVector2(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Vector3)
            {
                Vector3 currentValue = inValue.AsVector3;
                Vector3 nextValue = EditorGUILayout.Vector3Field(inLabel, currentValue);
                return RSValue.FromVector3(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Vector4)
            {
                Vector4 currentValue = inValue.AsVector4;
                Vector4 nextValue = EditorGUILayout.Vector4Field(inLabel, currentValue);
                return RSValue.FromVector4(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.Entity)
            {
                EntityScopeData currentValue = inValue.AsEntity;
                EntityScopeData nextValue = EntityScopeField(inLabel, currentValue, inFlags.ForEntityValue(), inContext);
                return RSValue.FromEntity(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.GroupId)
            {
                RSGroupId currentValue = inValue.AsGroupId;
                RSGroupId nextValue = LibraryGUILayout.GroupSelector(inLabel, currentValue, inContext.Library);
                return RSValue.FromGroupId(nextValue);
            }
            else if (inExpectedType == RSBuiltInTypes.TriggerId)
            {
                RSTriggerId currentValue = inValue.AsTriggerId;
                RSTypeInfo restrictTriggerType = inContext.Parameter?.TriggerParameterType;
                RSTriggerId nextValue;
                if (restrictTriggerType != null)
                    nextValue = LibraryGUILayout.TriggerSelector(inLabel, currentValue, restrictTriggerType, inContext.Library);
                else
                    nextValue = LibraryGUILayout.TriggerSelector(inLabel, currentValue, inContext.Library);
                return RSValue.FromTriggerId(nextValue);
            }
            else
            {
                EditorGUILayout.HelpBox(string.Format("Unable to display editor for type {0}", inExpectedType), MessageType.Error);
            }

            return inValue;
        }

        #endregion // RSValue

        #region NestedValue

        /// <summary>
        /// Renders a layout editor for a NestedValue.
        /// </summary>
        static public NestedValue NestedValueField(GUIContent inLabel, NestedValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoNestedValueField(inLabel, inValue, inExpectedType, inExpectedType != null ? inExpectedType.DefaultValue : RSValue.Null, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for a NestedValue.
        /// </summary>
        static public NestedValue NestedValueField(NestedValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoNestedValueField(GUIContent.none, inValue, inExpectedType, inExpectedType != null ? inExpectedType.DefaultValue : RSValue.Null, inFlags, inContext);
            }
        }

        /// <summary>
        /// Renders a layout editor for a NestedValue.
        /// </summary>
        static public NestedValue NestedValueField(GUIContent inLabel, NestedValue inValue, RSTypeInfo inExpectedType, RSValue inDefaultValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoNestedValueField(inLabel, inValue, inExpectedType, inDefaultValue, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for a NestedValue.
        /// </summary>
        static public NestedValue NestedValueField(NestedValue inValue, RSTypeInfo inExpectedType, RSValue inDefaultValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoNestedValueField(GUIContent.none, inValue, inExpectedType, inDefaultValue, inFlags, inContext);
            }
        }

        static private NestedValue DoNestedValueField(GUIContent inLabel, NestedValue inValue, RSTypeInfo inExpectedType, RSValue inDefaultValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            EditorGUILayout.BeginVertical();
            ResolvableValueMode nextType = ListGUILayout.Popup(inLabel, inValue.Mode, RSEditorUtility.GetResolvableValueModes(inExpectedType, inFlags, inContext));

            RSValue value = inDefaultValue;
            EntityScopedIdentifier query = new EntityScopedIdentifier(EntityScopeData.Self(), 0);
            RegisterIndex register = RegisterIndex.Register0;

            using(new EditorGUI.IndentLevelScope())
            {
                switch (inValue.Mode)
                {
                    case ResolvableValueMode.Argument:
                        {
                            if (inContext.Trigger == null)
                            {
                                EditorGUILayout.HelpBox("No parameter available: No Trigger", MessageType.Error);
                            }
                            else if (inContext.Trigger.ParameterType == null)
                            {
                                EditorGUILayout.HelpBox(string.Format("No parameter available - Trigger {0} has no parameter", inContext.Trigger.Name), MessageType.Error);
                            }
                            else if (inExpectedType != null && !inContext.Trigger.ParameterType.Type.CanConvert(inExpectedType))
                            {
                                EditorGUILayout.HelpBox(string.Format("No parameter available - Trigger {0} has incompatible parameter type {1}, which cannot convert to {2}", inContext.Trigger.Name, inContext.Trigger.ParameterType.Type, inExpectedType), MessageType.Error);
                            }
                            break;
                        }

                    case ResolvableValueMode.Value:
                        {
                            if (inExpectedType == null || inExpectedType == RSBuiltInTypes.Any || inFlags.Has(RSValidationFlags.DisallowDirectValue))
                            {
                                EditorGUILayout.HelpBox("Cannot specify a value in this context", MessageType.Error);
                            }
                            else
                            {
                                value = RSValueField(EditorGUIUtility.TrTempContent(inExpectedType.FriendlyName), inValue.Value, inExpectedType, inFlags, inContext);
                            }
                            break;
                        }

                    case ResolvableValueMode.Query:
                        {
                            query = ValueGUILayout.QueryField(RuleGUILayout.Content.ResolvableValueQueryLabel, inValue.Query, inExpectedType, inFlags.ForMethod(false), inContext);
                            break;
                        }

                    case ResolvableValueMode.Register:
                        {
                            register = (RegisterIndex) EnumGUILayout.EnumField(RuleGUILayout.Content.ResolvableValueRegisterLabel, inValue.Register);
                            break;
                        }
                }
            }

            EditorGUILayout.EndVertical();

            switch (nextType)
            {
                case ResolvableValueMode.Argument:
                    return NestedValue.FromArgument();

                case ResolvableValueMode.Query:
                    return NestedValue.FromQuery(query);

                case ResolvableValueMode.Register:
                    return NestedValue.FromRegister(register);

                case ResolvableValueMode.Value:
                default:
                    return NestedValue.FromValue(value);
            }
        }

        #endregion // NestedValue

        #region Query

        /// <summary>
        /// Renders a layout editor for a scoped query identifier.
        /// </summary>
        static public EntityScopedIdentifier QueryField(GUIContent inLabel, EntityScopedIdentifier inIdentifier, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoQueryField(inLabel, inIdentifier, inExpectedType, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for a scoped query identifier.
        /// </summary>
        static public EntityScopedIdentifier QueryField(EntityScopedIdentifier inIdentifier, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoQueryField(GUIContent.none, inIdentifier, inExpectedType, inFlags, inContext);
            }
        }

        static private EntityScopedIdentifier DoQueryField(GUIContent inLabel, EntityScopedIdentifier inIdentifier, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            EntityScopeData scope = inIdentifier.Scope;
            int queryId = inIdentifier.Id;
            bool bNoParams = inFlags.Has(RSValidationFlags.DisallowParameters);

            using(new EditorGUILayout.VerticalScope())
            {
                scope = EntityScopeField(inLabel, scope, inFlags.ForMethodScope(), inContext);
                using(new EditorGUI.IndentLevelScope())
                {
                    switch (scope.Type)
                    {
                        case EntityScopeType.Global:
                            queryId = LibraryGUILayout.QuerySelectorGlobal(Content.QueryIdLabel, queryId, inExpectedType, bNoParams, inContext.Library);
                            break;

                        case EntityScopeType.Null:
                            EditorGUILayout.HelpBox("Cannot perform query on null entity", MessageType.Error);
                            break;

                        case EntityScopeType.Invalid:
                            EditorGUILayout.HelpBox("Cannot perform query on missing entity", MessageType.Error);
                            break;

                        case EntityScopeType.Self:
                            {
                                if (inFlags.Has(RSValidationFlags.FilterSelection) && !scope.HasLinks() && inContext.Entity != null)
                                    queryId = LibraryGUILayout.QuerySelector(Content.QueryIdLabel, queryId, inContext.Entity, inExpectedType, bNoParams, inContext.Library);
                                else
                                    queryId = LibraryGUILayout.QuerySelectorUnknown(Content.QueryIdLabel, queryId, inExpectedType, bNoParams, inContext.Library);
                                break;
                            }

                        case EntityScopeType.ObjectById:
                            {
                                RSEntityId entityId = scope.IdArg;
                                IRSEntity entity = null;
                                if (inFlags.Has(RSValidationFlags.FilterSelection) && entityId != RSEntityId.Null && !scope.HasLinks() && inContext.Manager != null && (entity = inContext.Manager.Lookup.EntityWithId(entityId)) != null)
                                    queryId = LibraryGUILayout.QuerySelector(Content.QueryIdLabel, queryId, entity, inExpectedType, bNoParams, inContext.Library);
                                else
                                    queryId = LibraryGUILayout.QuerySelectorUnknown(Content.QueryIdLabel, queryId, inExpectedType, bNoParams, inContext.Library);
                                break;
                            }

                        default:
                            queryId = LibraryGUILayout.QuerySelectorUnknown(Content.QueryIdLabel, queryId, inExpectedType, bNoParams, inContext.Library);
                            break;
                    }

                    if (queryId == 0)
                        EditorGUILayout.HelpBox("Cannot perform null query", MessageType.Error);
                }
            }

            return new EntityScopedIdentifier(scope, queryId);
        }

        #endregion // Query

        #region Action

        /// <summary>
        /// Renders a layout editor for a scoped action identifier.
        /// </summary>
        static public EntityScopedIdentifier ActionField(GUIContent inLabel, EntityScopedIdentifier inIdentifier, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            return DoActionField(inLabel, inIdentifier, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for a scoped action identifier.
        /// </summary>
        static public EntityScopedIdentifier ActionField(EntityScopedIdentifier inIdentifier, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            using(new RSGUI.LabelWidthScope(0))
            {
                return DoActionField(GUIContent.none, inIdentifier, inFlags, inContext);
            }
        }

        static private EntityScopedIdentifier DoActionField(GUIContent inLabel, EntityScopedIdentifier inIdentifier, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            EntityScopeData scope = inIdentifier.Scope;
            int actionId = inIdentifier.Id;

            using(new EditorGUILayout.VerticalScope())
            {
                scope = EntityScopeField(inLabel, scope, inFlags.ForMethodScope(), inContext);
                using(new EditorGUI.IndentLevelScope())
                {
                    switch (scope.Type)
                    {
                        case EntityScopeType.Global:
                            actionId = LibraryGUILayout.ActionSelectorGlobal(Content.ActionIdLabel, actionId, inContext.Library);
                            break;

                        case EntityScopeType.Null:
                            EditorGUILayout.HelpBox("Cannot perform action on null entity", MessageType.Error);
                            break;

                        case EntityScopeType.Invalid:
                            EditorGUILayout.HelpBox("Cannot perform action on missing entity", MessageType.Error);
                            break;

                        case EntityScopeType.Self:
                            {
                                if (inFlags.Has(RSValidationFlags.FilterSelection) && !scope.HasLinks() && inContext.Entity != null)
                                    actionId = LibraryGUILayout.ActionSelector(Content.ActionIdLabel, actionId, inContext.Entity, inContext.Library);
                                else
                                    actionId = LibraryGUILayout.ActionSelectorUnknown(Content.ActionIdLabel, actionId, inContext.Library);
                                break;
                            }

                        case EntityScopeType.ObjectById:
                            {
                                RSEntityId entityId = scope.IdArg;
                                IRSEntity entity = null;
                                if (inFlags.Has(RSValidationFlags.FilterSelection) && entityId != RSEntityId.Null && !scope.HasLinks() && inContext.Manager != null && (entity = inContext.Manager.Lookup.EntityWithId(entityId)) != null)
                                    actionId = LibraryGUILayout.ActionSelector(Content.ActionIdLabel, actionId, entity, inContext.Library);
                                else
                                    actionId = LibraryGUILayout.ActionSelectorUnknown(Content.ActionIdLabel, actionId, inContext.Library);
                                break;
                            }

                        default:
                            actionId = LibraryGUILayout.ActionSelectorUnknown(Content.ActionIdLabel, actionId, inContext.Library);
                            break;
                    }

                    if (actionId == 0)
                        EditorGUILayout.HelpBox("Cannot perform null action", MessageType.Error);
                }
            }

            return new EntityScopedIdentifier(scope, actionId);
        }

        #endregion // Action

        #region Parameter

        /// <summary>
        /// Renders a layout editor for a NestedValue linked to a parameter.
        /// </summary>
        static public NestedValue NestedParameterField(RSParameterInfo inParameterInfo, NestedValue inValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            RSValidationFlags flags = inFlags.ForParameter(inParameterInfo);
            if (!inParameterInfo.NotNull)
                flags |= RSValidationFlags.AllowNullEntity;
            return NestedValueField(EditorGUIUtility.TrTextContent(inParameterInfo.Name, inParameterInfo.Tooltip), inValue, inParameterInfo.Type, inParameterInfo.Default, flags, inContext.WithParameter(inParameterInfo));
        }

        #endregion // Parameter
    }
}