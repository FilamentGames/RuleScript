using System;
using RuleScript.Data;
using RuleScript.Metadata;
using UnityEngine;

namespace RuleScript.Validation
{
    static internal class ValidationLogic
    {
        static internal void ValidateTable(RSRuleTableData inTable, RSValidationState ioState, RSValidationContext inContext)
        {
            if (inContext.Library == null)
            {
                ioState.Error("No library provided");
                return;
            }
            else if (!inContext.Library.IsLoaded())
            {
                ioState.Error("Library not fully loaded");
                return;
            }

            if (inTable == null || inTable.Rules == null || inTable.Rules.Length == 0)
            {
                return;
            }

            for (int i = 0; i < inTable.Rules.Length; ++i)
            {
                ioState.PushContext("Rule {0}: {1}", i, inTable.Rules[i]?.Name);
                ValidateRule(inTable.Rules[i], ioState, inContext);
                ioState.PopContext();
            }
        }

        static private void ValidateRule(RSRuleData inRule, RSValidationState ioState, RSValidationContext inContext)
        {
            if (inRule == null)
            {
                ioState.Error("Null rule");
                return;
            }

            ioState.PushContext("Trigger");
            RSTriggerInfo triggerInfo = ValidateTriggerId(inRule.TriggerId, RSValidationFlags.None, ioState, inContext);
            inContext = inContext.WithTrigger(triggerInfo);
            ioState.PopContext();

            if (inRule.Conditions != null && inRule.Conditions.Length > 0)
            {
                for (int i = 0; i < inRule.Conditions.Length; ++i)
                {
                    ioState.PushContext("Condition {0}", i);
                    ValidateCondition(inRule.Conditions[i], ioState, inContext);
                    ioState.PopContext();
                }
            }

            if (inRule.Actions != null && inRule.Actions.Length > 0)
            {
                for (int i = 0; i < inRule.Actions.Length; ++i)
                {
                    ioState.PushContext("Action {0}", i);
                    ValidateAction(inRule.Actions[i], ioState, inContext);
                    ioState.PopContext();
                }
            }
        }

        static private void ValidateCondition(RSConditionData inCondition, RSValidationState ioState, RSValidationContext inContext)
        {
            ioState.PushContext("Query");
            ValidateResolvableValue(inCondition.Query, null, RSValidationFlags.None.ForConditionQuery(), ioState, inContext);
            ioState.PopContext();

            RSTypeInfo expectedType = inCondition.Query.TypeInfo(inContext.Trigger, inContext.Library);
            if (inCondition.Query.Mode != ResolvableValueMode.Value && expectedType != null)
            {
                ioState.PushContext("Operator");
                CompareOperator op = inCondition.Operator;
                if (!expectedType.IsOperatorAllowed(op))
                {
                    ioState.Error("Operator {0} is not allowed for type {1}", op, expectedType);
                }
                ioState.PopContext();

                if (op.IsBinary())
                {
                    ioState.PushContext("Target");
                    ValidateResolvableValue(inCondition.Target, expectedType, RSValidationFlags.None.ForConditionTarget(), ioState, inContext);
                    ioState.PopContext();
                }
            }
        }

        static private void ValidateAction(RSActionData inAction, RSValidationState ioState, RSValidationContext inContext)
        {
            ioState.PushContext("Action Id");
            RSActionInfo actionInfo = ValidateActionId(inAction.Action, RSValidationFlags.None, ioState, inContext);
            ioState.PopContext();

            ioState.PushContext("Arguments");
            if (actionInfo != null)
            {
                int argCount = actionInfo.Parameters.Length;
                if (argCount <= 0)
                {
                    if (inAction.Arguments != null && inAction.Arguments.Length > 0)
                        ioState.Error("Arguments provided for action {0} but none required", actionInfo.Name);
                }
                else
                {
                    if (inAction.Arguments == null)
                    {
                        ioState.Error("No arguments provided for action {0} but {1} required", actionInfo.Name, argCount);
                    }
                    else if (inAction.Arguments.Length != argCount)
                    {
                        ioState.Error("Argument count mismatch for action {0} - {1} required but {2} provided", actionInfo.Name, argCount, inAction.Arguments.Length);
                    }
                    else
                    {
                        for (int i = 0; i < argCount; ++i)
                        {
                            ValidateParameter(actionInfo.Parameters[i], inAction.Arguments[i], ioState, inContext);
                        }
                    }
                }
            }
            ioState.PopContext();
        }

        static private void ValidateParameter(RSParameterInfo inParameterInfo, RSResolvableValueData inValue, RSValidationState ioState, RSValidationContext inContext)
        {
            ioState.PushContext("Parameter: {0}", inParameterInfo.Name);
            ValidateResolvableValue(inValue, inParameterInfo.Type, RSValidationFlags.None.ForParameter(inParameterInfo), ioState, inContext.WithParameter(inParameterInfo));
            ioState.PopContext();
        }

        static private void ValidateNestedParameter(RSParameterInfo inParameterInfo, NestedValue inValue, RSValidationState ioState, RSValidationContext inContext)
        {
            ioState.PushContext("Parameter: {0}", inParameterInfo.Name);
            ValidateNestedValue(inValue, inParameterInfo.Type, RSValidationFlags.None.ForParameter(inParameterInfo), ioState, inContext.WithParameter(inParameterInfo));
            ioState.PopContext();
        }

        static private void ValidateResolvableValue(RSResolvableValueData inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            bool bDisallowDirectValue = (inExpectedType == null || inExpectedType == RSBuiltInTypes.Any || inFlags.Has(RSValidationFlags.DisallowDirectValue));
            switch (inValue.Mode)
            {
                case ResolvableValueMode.Argument:
                    {
                        ValidateTriggerArgument(inExpectedType, inFlags, ioState, inContext);
                        break;
                    }

                case ResolvableValueMode.Value:
                    {
                        if (bDisallowDirectValue)
                        {
                            ioState.Error("Cannot specify a direct value in this context");
                        }
                        else
                        {
                            ValidateValue(inValue.Value, inExpectedType, inFlags, ioState, inContext);
                        }
                        break;
                    }

                case ResolvableValueMode.Register:
                    {
                        if (inFlags.Has(RSValidationFlags.DisallowRegisters))
                        {
                            ioState.Error("Cannot use a register in this context");
                        }
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        ioState.PushContext("Query Id");
                        RSQueryInfo queryInfo = ValidateQueryId(inValue.Query, inExpectedType, inFlags.ForMethod(true), ioState, inContext);
                        ioState.PopContext();

                        ioState.PushContext("Arguments");
                        if (queryInfo != null)
                        {
                            int argCount = queryInfo.Parameters.Length;
                            if (argCount <= 0)
                            {
                                if (inValue.QueryArguments != null && inValue.QueryArguments.Length > 0)
                                    ioState.Error("Arguments provided for action {0} but none required", queryInfo.Name);
                            }
                            else
                            {
                                if (inValue.QueryArguments == null)
                                {
                                    ioState.Error("No arguments provided for action {0} but {1} required", queryInfo.Name, argCount);
                                }
                                else if (inValue.QueryArguments.Length != argCount)
                                {
                                    ioState.Error("Argument count mismatch for action {0} - {1} required but {2} provided", queryInfo.Name, argCount, inValue.QueryArguments.Length);
                                }
                                else
                                {
                                    for (int i = 0; i < argCount; ++i)
                                    {
                                        ValidateNestedParameter(queryInfo.Parameters[i], inValue.QueryArguments[i], ioState, inContext);
                                    }
                                }
                            }
                        }
                        ioState.PopContext();
                        break;
                    }
            }
        }

        static private void ValidateNestedValue(NestedValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            bool bDisallowDirectValue = (inExpectedType == null || inExpectedType == RSBuiltInTypes.Any || inFlags.Has(RSValidationFlags.DisallowDirectValue));
            switch (inValue.Mode)
            {
                case ResolvableValueMode.Argument:
                    {
                        ValidateTriggerArgument(inExpectedType, inFlags, ioState, inContext);
                        break;
                    }

                case ResolvableValueMode.Register:
                    {
                        if (inFlags.Has(RSValidationFlags.DisallowRegisters))
                        {
                            ioState.Error("Cannot use a register in this context");
                        }
                        break;
                    }

                case ResolvableValueMode.Value:
                    {
                        if (bDisallowDirectValue)
                        {
                            ioState.Error("Cannot specify a direct value in this context");
                        }
                        else
                        {
                            ValidateValue(inValue.Value, inExpectedType, inFlags, ioState, inContext);
                        }
                        break;
                    }

                case ResolvableValueMode.Query:
                    {
                        ValidateQueryId(inValue.Query, inExpectedType, inFlags.ForMethod(false), ioState, inContext);
                        break;
                    }
            }
        }

        static private void ValidateEntityScope(EntityScopeData inScope, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            bool bForceFirst = inFlags.Has(RSValidationFlags.RequireSingleEntity);

            switch (inScope.Type)
            {
                case EntityScopeType.ObjectById:
                    {
                        RSEntityId entityId = inScope.IdArg;
                        ValidateEntityId(entityId, inFlags, ioState, inContext);
                        break;
                    }

                case EntityScopeType.ObjectsWithGroup:
                    {
                        if (bForceFirst && !inScope.UseFirst)
                        {
                            ioState.Error("Potentially multiple return values in context where only one value is accepted");
                        }

                        ValidateGroupId(inScope.GroupArg, inFlags, ioState, inContext);
                        break;
                    }

                case EntityScopeType.ObjectsWithName:
                case EntityScopeType.ObjectsWithPrefab:
                    {
                        if (bForceFirst && !inScope.UseFirst)
                        {
                            ioState.Error("Potentially multiple return values in context where only one value is accepted");
                        }

                        string name = inScope.SearchArg;
                        if (string.IsNullOrEmpty(name))
                        {
                            ioState.Warn("Empty search string");
                        }
                        break;
                    }

                case EntityScopeType.Null:
                    {
                        if (!inFlags.Has(RSValidationFlags.AllowNullEntity))
                        {
                            ioState.Error("Null entity not allowed in this context");
                        }
                        break;
                    }

                case EntityScopeType.Invalid:
                    {
                        ioState.Error("Missing entity");
                        break;
                    }

                case EntityScopeType.Global:
                    {
                        if (!inFlags.Has(RSValidationFlags.AllowGlobalEntity))
                        {
                            ioState.Error("Global entity not allowed in this context");
                        }
                        break;
                    }

                case EntityScopeType.Argument:
                    {
                        ValidateTriggerArgument(RSBuiltInTypes.Entity, inFlags, ioState, inContext);
                        break;
                    }
            }
        }

        static private void ValidateValue(RSValue inValue, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            Type systemType = inExpectedType.SystemType;
            if (systemType.IsEnum)
            {
                try
                {
                    Enum currentValue = inValue.AsEnum();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    ioState.Error("Enum {0} cannot be represented as type {1}", inValue, inExpectedType);
                }
                return;
            }

            if (inExpectedType == RSBuiltInTypes.Entity)
            {
                EntityScopeData scope = inValue.AsEntity;
                ValidateEntityScope(scope, inFlags.ForEntityValue(), ioState, inContext);
            }
            else if (inExpectedType == RSBuiltInTypes.GroupId)
            {
                RSGroupId group = inValue.AsGroupId;
                ValidateGroupId(group, inFlags, ioState, inContext);
            }
            else if (inExpectedType == RSBuiltInTypes.TriggerId)
            {
                RSTriggerId triggerId = inValue.AsTriggerId;
                ValidateTriggerId(triggerId, inFlags, ioState, inContext);
            }
        }

        static private RSGroupInfo ValidateGroupId(RSGroupId inGroupId, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            if (inGroupId != RSGroupId.Null)
            {
                RSGroupInfo groupInfo = inContext.Library.GetGroup(inGroupId);
                if (groupInfo == null)
                {
                    ioState.Error("Group {0} does not exist", inGroupId);
                }

                return groupInfo;
            }
            else
            {
                return null;
            }
        }

        static private RSTriggerInfo ValidateTriggerId(RSTriggerId inTriggerId, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            RSTypeInfo restrictTriggerType = inContext.Parameter?.TriggerParameterType;
            if (inTriggerId == RSTriggerId.Null)
            {
                if (restrictTriggerType != null)
                    ioState.Error("Null trigger id provided - require trigger with parameter type {0}", restrictTriggerType);
                else
                    ioState.Warn("Null trigger provided");
                return null;
            }
            else
            {
                RSTriggerInfo triggerInfo = inContext.Library.GetTrigger(inTriggerId);
                if (triggerInfo == null)
                {
                    ioState.Error("Trigger {0} does not exist", inTriggerId);
                }
                else
                {
                    if (restrictTriggerType != null)
                    {
                        if (restrictTriggerType == RSBuiltInTypes.Void)
                        {
                            if (triggerInfo.ParameterType != null)
                                ioState.Error("Trigger with no parameter required, but trigger {0} with parameter {1} provided", triggerInfo.Name, triggerInfo.ParameterType.Type);
                        }
                        else
                        {
                            if (triggerInfo.ParameterType == null)
                            {
                                ioState.Error("Trigger with parameter {0} required, but trigger {1} with no parameter provided", restrictTriggerType, triggerInfo.Name);
                            }
                            else if (!restrictTriggerType.CanConvert(triggerInfo.ParameterType.Type))
                            {
                                ioState.Error("Trigger with parameter {0} required, but trigger {1} with incompatible parameter {2} provided", restrictTriggerType, triggerInfo.Name, triggerInfo.ParameterType.Type);
                            }
                        }
                    }
                }
                return triggerInfo;
            }
        }

        static private void ValidateEntityId(RSEntityId inEntityId, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            if (inEntityId == RSEntityId.Null && !inFlags.Has(RSValidationFlags.AllowNullEntity))
            {
                ioState.Error("Null entity not allowed in this context");
            }
            else if (inEntityId != RSEntityId.Null)
            {
                if (inContext.Manager != null)
                {
                    var entity = inContext.Manager.Lookup.EntityWithId(inEntityId);
                    if (entity == null)
                        ioState.Error("No entity with id {0} found in entity manager", inEntityId);
                }
                else
                {
                    ioState.Warn("No entity manager found - unable to verify that entity with id {0} exists", inEntityId);
                }
            }
        }

        static private void ValidateTriggerArgument(RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            if (inContext.Trigger == null)
            {
                ioState.Error("Cannot use trigger parameter - no trigger");
            }
            else if (inContext.Trigger.ParameterType == null)
            {
                ioState.Error("Cannot use trigger parameter - trigger {0} has no parameter", inContext.Trigger.Name);
            }
            else if (inExpectedType != null && !inContext.Trigger.ParameterType.Type.CanConvert(inExpectedType))
            {
                ioState.Error("Cannot use trigger parameter - trigger {0} has incompatible parameter type {1}, which cannot convert to {2}", inContext.Trigger.Name, inContext.Trigger.ParameterType.Type, inExpectedType);
            }
        }

        static private RSQueryInfo ValidateQueryId(EntityScopedIdentifier inIdentifier, RSTypeInfo inExpectedType, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            bool bNoParams = inFlags.Has(RSValidationFlags.DisallowParameters);

            ValidateEntityScope(inIdentifier.Scope, inFlags.ForMethodScope(), ioState, inContext);

            if (inIdentifier.Id == 0)
            {
                ioState.Error("Null query not allowed");
                return null;
            }
            else
            {
                RSQueryInfo queryInfo = inContext.Library.GetQuery(inIdentifier.Id);
                if (queryInfo == null)
                {
                    ioState.Error("Query {0} does not exist", inIdentifier.Id);
                }
                else
                {
                    if (inExpectedType != null && !queryInfo.ReturnType.CanConvert(inExpectedType))
                    {
                        ioState.Error("Query {0} returns incompatible type {1}, which cannot convert to desired type {2}", queryInfo.Name, queryInfo.ReturnType, inExpectedType);
                    }

                    if (bNoParams && queryInfo.Parameters != null && queryInfo.Parameters.Length > 0)
                    {
                        ioState.Error("Query {0} has parameters, which is not allowed in this context", queryInfo.Name);
                    }

                    switch (inIdentifier.Scope.Type)
                    {
                        case EntityScopeType.Global:
                            {
                                if (queryInfo.OwnerType != null)
                                    ioState.Error("Query {0} is bound to type {1} but was specified as a global query", queryInfo.Name, queryInfo.OwnerType.Name);
                                break;
                            }

                        case EntityScopeType.Null:
                        case EntityScopeType.Invalid:
                            break;

                        default:
                            {
                                if (queryInfo.OwnerType == null)
                                    ioState.Error("Query {0} is bound to global scope but was specified as a local query", queryInfo.Name);
                                break;
                            }
                    }
                }

                return queryInfo;
            }
        }

        static private RSActionInfo ValidateActionId(EntityScopedIdentifier inIdentifier, RSValidationFlags inFlags, RSValidationState ioState, RSValidationContext inContext)
        {
            ValidateEntityScope(inIdentifier.Scope, inFlags.ForMethodScope(), ioState, inContext);

            if (inIdentifier.Id == 0)
            {
                ioState.Error("Null action not allowed");
                return null;
            }
            else
            {
                RSActionInfo actionInfo = inContext.Library.GetAction(inIdentifier.Id);
                if (actionInfo == null)
                {
                    ioState.Error("Action {0} does not exist", inIdentifier.Id);
                }
                else
                {
                    switch (inIdentifier.Scope.Type)
                    {
                        case EntityScopeType.Global:
                            {
                                if (actionInfo.OwnerType != null)
                                    ioState.Error("Action {0} is bound to type {1} but was specified as a global action", actionInfo.Name, actionInfo.OwnerType.Name);
                                break;
                            }

                        case EntityScopeType.Null:
                        case EntityScopeType.Invalid:
                            break;

                        default:
                            {
                                if (actionInfo.OwnerType == null)
                                    ioState.Error("Action {0} is bound to global scope but was specified as a local action", actionInfo.Name);
                                break;
                            }
                    }
                }

                return actionInfo;
            }
        }
    }
}