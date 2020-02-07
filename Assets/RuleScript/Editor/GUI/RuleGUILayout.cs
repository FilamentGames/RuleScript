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
    static public partial class RuleGUILayout
    {
        #region Content

        static public class Content
        {
            // Rule
            static public readonly GUIContent RuleNameLabel = new GUIContent("Name", "Name of the rule\nRules with the same name can be activated, deactivated, and stopped in RuleScript as a group.");
            static public readonly GUIContent RuleTriggerLabel = new GUIContent("Trigger", "Trigger for this rule's evaluation\nRules are only evaluated when this trigger is received.");
            static public readonly GUIContent RuleParameterLabel = new GUIContent("Parameter", "The parameter dispatched with this trigger\nSome triggers also send information to entities.");
            static public readonly GUIContent RuleEnabledLabel = new GUIContent("Enabled", "Whether or not this rule is enabled\nDisabled rules are not evalated when a trigger is recieved.\nYou can enable rules by name in RuleScript.");
            static public readonly GUIContent RuleOnlyOnceLabel = new GUIContent("Only Once?", "If checked, the rule will disable itself once executed\nYou can reenable a rule by name from RuleScript");
            static public readonly GUIContent RuleDontInterruptLabel = new GUIContent("Don't Interrupt", "If checked, this rule will not be evaluated if it is already running.\nWhile only one instance of a rule can be evaluating at a time on an entity, this will prevent that instance from being interrupted by the same conditions.");
            static public readonly GUIContent RuleGroupLabel = new GUIContent("Group", "If set, only one rule with this group can be executing at any given time for on the entity");

            // Condition
            static public readonly GUIContent ConditionEnabledLabel = new GUIContent("Enabled", "Whether or not this condition should be evaluated.");
            static public readonly GUIContent ConditionValueLabel = new GUIContent("Value 1", "The first value");
            static public readonly GUIContent ConditionComparisonLabel = new GUIContent("Comparison", "A comparison to be made");
            static public readonly GUIContent ConditionTargetLabel = new GUIContent("Value 2", "The second value\nThe first value is compared to this one with the comparison operator.");
            static public readonly GUIContent ConditionSubsetLabel = new GUIContent("Subset", "The first value can return values from multiple entities\nWhat subset of those values must pass the comparison?");

            // ResolvableValue
            static public readonly GUIContent ResolvableValueRegisterLabel = new GUIContent("Variable Slot", "Which local variable from which to retrieve a value");
            static public readonly GUIContent ResolvableValueQueryLabel = new GUIContent("Query", "Which entity and query to call to retrieve a value");
            static public readonly GUIContent ResolvableValueQueryArgsLabel = new GUIContent("Query Arguments", "Arguments to pass to the query");
        }

        #endregion // Content

        #region Basics

        /// <summary>
        /// Renders editor for rule info. Does not include conditions or actions.
        /// </summary>
        static public void RuleData(UndoTarget inUndo, RSRuleData ioRule, RSRuleTableData ioTable, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            string preview = ioRule.GetPreviewString(null, inContext.Library);
            GUILayout.Label(preview, RSGUIStyles.RuleHeaderStyle);

            EditorGUILayout.Space();

            string newName = EditorGUILayout.TextField(Content.RuleNameLabel, ioRule.Name);
            if (newName != ioRule.Name)
            {
                inUndo.MarkDirty("Changed Rule Name");
                ioRule.Name = newName;
            }

            RSTriggerId newTriggerId;
            if (inFlags.Has(RSValidationFlags.FilterSelection) && inContext.Entity != null)
                newTriggerId = LibraryGUILayout.TriggerSelector(Content.RuleTriggerLabel, ioRule.TriggerId, inContext.Entity, inContext.Library);
            else
                newTriggerId = LibraryGUILayout.TriggerSelector(Content.RuleTriggerLabel, ioRule.TriggerId, inContext.Library);

            if (newTriggerId != ioRule.TriggerId)
            {
                inUndo.MarkDirty("Changed Rule Trigger", true);
                ioRule.TriggerId = newTriggerId;

                TableUtils.UpdateUniqueRuleTriggers(ioTable);
            }

            if (newTriggerId != RSTriggerId.Null)
            {
                RSTriggerInfo info = inContext.Library.GetTrigger(newTriggerId);
                if (info != null && info.ParameterType != null)
                {
                    using(new EditorGUI.IndentLevelScope())
                    {
                        EditorGUILayout.LabelField(Content.RuleParameterLabel, EditorGUIUtility.TrTextContent(info.ParameterType.ToStringWithoutDefault(), info.ParameterType.Tooltip));
                    }
                }
            }

            EditorGUILayout.Space();

            // Enabled
            bool bEnabled = EditorGUILayout.Toggle(Content.RuleEnabledLabel, ioRule.Enabled);
            if (bEnabled != ioRule.Enabled)
            {
                inUndo.MarkDirty("Changed Rule Enabled");
                ioRule.Enabled = bEnabled;
            }

            bool bOnlyOnce = EditorGUILayout.Toggle(Content.RuleOnlyOnceLabel, ioRule.OnlyOnce);
            if (bOnlyOnce != ioRule.OnlyOnce)
            {
                inUndo.MarkDirty("Changed Rule OnlyOnce");
                ioRule.OnlyOnce = bOnlyOnce;
            }

            bool bDontInterrupt = EditorGUILayout.Toggle(Content.RuleDontInterruptLabel, ioRule.DontInterrupt);
            if (bDontInterrupt != ioRule.DontInterrupt)
            {
                inUndo.MarkDirty("Changed Rule DontInterrupt");
                ioRule.DontInterrupt = bDontInterrupt;
            }

            EditorGUILayout.Space();

            string newGroup = EditorGUILayout.TextField(Content.RuleGroupLabel, ioRule.RoutineGroup);
            if (newGroup != ioRule.RoutineGroup)
            {
                inUndo.MarkDirty("Changed Rule Group");
                ioRule.RoutineGroup = newGroup;
            }
        }

        /// <summary>
        /// Renders editor for condition info.
        /// </summary>
        static public void ConditionData(UndoTarget inUndo, RSConditionData ioCondition, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            string preview = ioCondition.GetPreviewString(inContext.Trigger, inContext.Library);
            GUILayout.Label(preview, RSGUIStyles.RuleHeaderStyle);

            EditorGUILayout.Space();

            // Enabled
            bool bEnabled = EditorGUILayout.Toggle(Content.ConditionEnabledLabel, ioCondition.Enabled);
            if (bEnabled != ioCondition.Enabled)
            {
                inUndo.MarkDirty("Changed Condition Enabled");
                ioCondition.Enabled = bEnabled;
            }

            EditorGUILayout.Space();

            using(new EditorGUI.DisabledGroupScope(!bEnabled))
            {
                RSTypeInfo prevQueryType = ioCondition.Query.TypeInfo(inContext.Trigger, inContext.Library);

                // Query
                ResolvableValueData(inUndo, Content.ConditionValueLabel, ioCondition.Query, null, inFlags.ForConditionQuery(), inContext);

                // comparison
                RSTypeInfo queryTypeInfo = ioCondition.Query.TypeInfo(inContext.Trigger, inContext.Library);
                if (ioCondition.Query.Mode != ResolvableValueMode.Value && queryTypeInfo != null)
                {
                    EditorGUILayout.Space();

                    RSEditorUtility.s_ComparisonOperators.Clear();
                    foreach (var comparison in queryTypeInfo.AllowedOperators())
                    {
                        RSEditorUtility.s_ComparisonOperators.Add(comparison, comparison.Name(), (int) comparison);
                    }

                    CompareOperator nextOperator = ioCondition.Operator;
                    if (!RSEditorUtility.s_ComparisonOperators.Contains(nextOperator))
                        nextOperator = RSEditorUtility.s_ComparisonOperators.Get(0);

                    nextOperator = ListGUILayout.Popup(Content.ConditionComparisonLabel, nextOperator, RSEditorUtility.s_ComparisonOperators);
                    if (nextOperator != ioCondition.Operator)
                    {
                        inUndo.MarkDirty("Changed Condition Operator");
                        ioCondition.Operator = nextOperator;
                    }

                    if (nextOperator.IsBinary())
                    {
                        EditorGUILayout.Space();

                        if (prevQueryType != queryTypeInfo)
                        {
                            inUndo.MarkDirty("Changed Condition Query Type");
                            RSResolvableValueData.SetAsValue(ref ioCondition.Target, queryTypeInfo.DefaultValue);
                        }

                        ResolvableValueData(inUndo, Content.ConditionTargetLabel, ioCondition.Target, queryTypeInfo, inFlags.ForConditionTarget(), inContext);
                    }
                    else
                    {
                        if (ioCondition.Target != null)
                        {
                            inUndo.MarkDirty("Removed Condition Comparison Target");
                            ioCondition.Target = null;
                        }
                    }

                    if (ioCondition.Query.IsMultiValue())
                    {
                        EditorGUILayout.Space();
                        Subset subset = (Subset) EditorGUILayout.EnumPopup(Content.ConditionSubsetLabel, ioCondition.MultiQuerySubset);
                        if (subset != ioCondition.MultiQuerySubset)
                        {
                            inUndo.MarkDirty("Changed Condition MultiQuerySubset");
                            ioCondition.MultiQuerySubset = subset;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Renders editor for Action info.
        /// </summary>
        static public void ActionData(UndoTarget inUndo, RSActionData ioAction, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            string preview = ioAction.GetPreviewString(inContext.Trigger, inContext.Library);
            GUILayout.Label(preview, RSGUIStyles.RuleHeaderStyle);

            EditorGUILayout.Space();

            // Enabled
            bool bEnabled = EditorGUILayout.Toggle("Enabled", ioAction.Enabled);
            if (bEnabled != ioAction.Enabled)
            {
                inUndo.MarkDirty("Changed Action Enabled");
                ioAction.Enabled = bEnabled;
            }

            EditorGUILayout.Space();

            using(new EditorGUI.DisabledGroupScope(!bEnabled))
            {
                EntityScopedIdentifier action = ValueGUILayout.ActionField(EditorGUIUtility.TrTempContent("Action"), ioAction.Action, inFlags, inContext);
                RSActionInfo actionInfo = inContext.Library.GetAction(action.Id);

                if (action != ioAction.Action)
                {
                    bool bChangedId = ioAction.Action.Id != action.Id;

                    inUndo.MarkDirty("Changed Action", true);
                    ioAction.Action = action;

                    if (bChangedId)
                    {
                        if (actionInfo != null)
                            actionInfo.PopulateDefaultArguments(ioAction);
                        else
                            ioAction.Arguments = null;
                    }
                }

                if (actionInfo != null && ioAction.Arguments != null && ioAction.Arguments.Length > 0)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Arguments", RSGUIStyles.SubHeaderStyle);

                    for (int i = 0; i < ioAction.Arguments.Length; ++i)
                    {
                        ParameterData(inUndo, actionInfo.Parameters[i], ioAction.Arguments[i], inFlags, inContext);
                    }
                }
            }
        }

        #endregion // Basics

        #region ResolvableValue

        /// <summary>
        /// Renders a layout editor for a ResolvableValue linked to a parameter.
        /// </summary>
        static public void ParameterData(UndoTarget inUndo, RSParameterInfo inParameterInfo, RSResolvableValueData ioValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            inFlags = inFlags.ForParameter(inParameterInfo);
            ResolvableValueData(inUndo, EditorGUIUtility.TrTextContent(inParameterInfo.Name, inParameterInfo.Tooltip), ioValue, inParameterInfo.Type, inParameterInfo.Default, inFlags, inContext.WithParameter(inParameterInfo));
        }

        /// <summary>
        /// Renders a layout editor for a ResolvableValue.
        /// </summary>
        static public void ResolvableValueData(UndoTarget inUndo, GUIContent inLabel, RSResolvableValueData ioValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            DoResolvableValueData(inUndo, inLabel, ioValue, inExpectedType, inExpectedType != null ? inExpectedType.DefaultValue : RSValue.Null, inFlags, inContext);
        }

        /// <summary>
        /// Renders a layout editor for a ResolvableValue.
        /// </summary>
        static public void ResolvableValueData(UndoTarget inUndo, GUIContent inLabel, RSResolvableValueData ioValue, RSTypeInfo inExpectedType, RSValue inDefaultValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            DoResolvableValueData(inUndo, inLabel, ioValue, inExpectedType, inDefaultValue, inFlags, inContext);
        }

        static private void DoResolvableValueData(UndoTarget inUndo, GUIContent inLabel, RSResolvableValueData ioValue, RSTypeInfo inExpectedType, RSValue inDefaultValue, RSValidationFlags inFlags, RSValidationContext inContext)
        {
            EditorGUILayout.BeginVertical();

            bool bDisallowDirectValue = (inExpectedType == null || inExpectedType == RSBuiltInTypes.Any || inFlags.Has(RSValidationFlags.DisallowDirectValue));

            ResolvableValueMode nextMode = ListGUILayout.Popup(inLabel, ioValue.Mode, RSEditorUtility.GetResolvableValueModes(inExpectedType, inFlags, inContext));
            if (nextMode != ioValue.Mode)
            {
                inUndo.MarkDirty("Changed Resolvable Value Mode");
                ioValue.Mode = nextMode;

                switch (nextMode)
                {
                    case ResolvableValueMode.Argument:
                        RSResolvableValueData.SetAsArgument(ref ioValue);
                        break;

                    case ResolvableValueMode.Query:
                        RSResolvableValueData.SetAsQuery(ref ioValue, new EntityScopedIdentifier(EntityScopeData.Self(), 0));
                        break;

                    case ResolvableValueMode.Value:
                        RSResolvableValueData.SetAsValue(ref ioValue, inDefaultValue);
                        break;

                    case ResolvableValueMode.Register:
                        RSResolvableValueData.SetAsRegister(ref ioValue, RegisterIndex.Register0);
                        break;
                }
            }

            using(new EditorGUI.IndentLevelScope())
            {
                switch (ioValue.Mode)
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
                            if (bDisallowDirectValue)
                            {
                                EditorGUILayout.HelpBox("Cannot specify a value in this context", MessageType.Error);
                            }
                            else
                            {
                                RSValue nextValue = ValueGUILayout.RSValueField(EditorGUIUtility.TrTempContent(inExpectedType.FriendlyName), ioValue.Value, inExpectedType, inFlags, inContext);
                                if (nextValue != ioValue.Value)
                                {
                                    inUndo.MarkDirty("Changed Resolvable Value Value");
                                    ioValue.Value = nextValue;
                                }
                            }
                            break;
                        }

                    case ResolvableValueMode.Register:
                        {
                            RegisterIndex nextRegister = (RegisterIndex) EnumGUILayout.EnumField(Content.ResolvableValueRegisterLabel, ioValue.Register);
                            if (nextRegister != ioValue.Register)
                            {
                                inUndo.MarkDirty("Changed Resolvable Value Register");
                                ioValue.Register = nextRegister;
                            }
                            break;
                        }

                    case ResolvableValueMode.Query:
                        {
                            EntityScopedIdentifier query = ValueGUILayout.QueryField(Content.ResolvableValueQueryLabel, ioValue.Query, inExpectedType, inFlags.ForMethod(true), inContext);
                            RSQueryInfo queryInfo = inContext.Library.GetQuery(query.Id);
                            if (query != ioValue.Query)
                            {
                                bool bChangedId = query.Id != ioValue.Query.Id;
                                inUndo.MarkDirty("Changed Resolvable Value Query", true);
                                ioValue.Query = query;

                                if (bChangedId)
                                {
                                    if (queryInfo == null)
                                    {
                                        ioValue.QueryArguments = null;
                                    }
                                    else
                                    {
                                        queryInfo.PopulateDefaultArguments(ioValue);
                                    }
                                }
                            }

                            int currentArgsLength = 0;
                            if (ioValue.QueryArguments != null)
                                currentArgsLength = ioValue.QueryArguments.Length;
                            int desiredArgsLength = 0;
                            if (queryInfo != null && queryInfo.Parameters != null)
                                desiredArgsLength = queryInfo.Parameters.Length;

                            if (desiredArgsLength == 0 && ioValue.QueryArguments != null)
                            {
                                inUndo.MarkDirtyWithoutUndo("Resizing Arguments", true);
                                ioValue.QueryArguments = null;
                            }
                            else if (desiredArgsLength > 0 && currentArgsLength != desiredArgsLength)
                            {
                                inUndo.MarkDirtyWithoutUndo("Resizing Arguments", true);
                                queryInfo.PopulateDefaultArguments(ioValue, currentArgsLength);
                            }

                            if (ioValue.QueryArguments != null && ioValue.QueryArguments.Length > 0)
                            {
                                using(new EditorGUI.IndentLevelScope())
                                {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.LabelField(Content.ResolvableValueQueryArgsLabel, RSGUIStyles.SubHeaderStyle);
                                    for (int i = 0; i < ioValue.QueryArguments.Length && i < queryInfo.Parameters.Length; ++i)
                                    {
                                        NestedValue nextValue = ValueGUILayout.NestedParameterField(queryInfo.Parameters[i], ioValue.QueryArguments[i], inFlags, inContext);
                                        if (nextValue != ioValue.QueryArguments[i])
                                        {
                                            inUndo.MarkDirty("Changed Resolvable Value Query Argument");
                                            ioValue.QueryArguments[i] = nextValue;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                }
            }

            EditorGUILayout.EndVertical();
        }

        #endregion // ResolvableValue
    }
}