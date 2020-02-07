using System;
using System.Collections.Generic;
using System.IO;
using BeauData;
using BeauUtil.Editor;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Runtime;
using UnityEditor;
using UnityEngine;

namespace RuleScript.Editor
{
    static internal class RSEditorClipboard
    {
        internal enum Target
        {
            None,
            Rule,
            Condition,
            Action
        }

        static private Target s_CurrentTarget;
        static private RSRuleData s_CurrentRule;
        static private RSConditionData s_CurrentCondition;
        static private RSActionData s_CurrentAction;

        static public void Clear()
        {
            s_CurrentTarget = Target.None;
            s_CurrentRule = null;
            s_CurrentCondition = null;
            s_CurrentAction = null;
        }

        #region Rule

        static public bool HasRule()
        {
            return s_CurrentTarget == Target.Rule && s_CurrentRule != null;
        }

        static public void CopyRule(RSRuleData inRuleData)
        {
            Clear();

            s_CurrentTarget = Target.Rule;
            s_CurrentRule = inRuleData.Clone();
        }

        static public RSRuleData PasteRule()
        {
            if (!HasRule())
            {
                Debug.LogError("No rule copied");
                return null;
            }

            return s_CurrentRule.Clone();
        }

        static public void PasteRule(RSRuleData ioTarget)
        {
            if (!HasRule())
            {
                Debug.LogError("No rule copied");
                return;
            }

            ioTarget.CopyFrom(s_CurrentRule);
        }

        #endregion // Rule

        #region Condition

        static public bool HasCondition()
        {
            return s_CurrentTarget == Target.Condition && s_CurrentCondition != null;
        }

        static public void CopyCondition(RSConditionData inConditionData)
        {
            Clear();

            s_CurrentTarget = Target.Condition;
            s_CurrentCondition = inConditionData.Clone();
        }

        static public RSConditionData PasteCondition()
        {
            if (!HasCondition())
            {
                Debug.LogError("No condition copied");
                return null;
            }

            return s_CurrentCondition.Clone();
        }

        static public void PasteCondition(RSConditionData ioTarget)
        {
            if (!HasCondition())
            {
                Debug.LogError("No condition copied");
                return;
            }

            ioTarget.CopyFrom(s_CurrentCondition);
        }

        #endregion // Condition

        #region Action

        static public bool HasAction()
        {
            return s_CurrentTarget == Target.Action && s_CurrentAction != null;
        }

        static public void CopyAction(RSActionData inActionData)
        {
            Clear();

            s_CurrentTarget = Target.Action;
            s_CurrentAction = inActionData.Clone();
        }

        static public RSActionData PasteAction()
        {
            if (!HasAction())
            {
                Debug.LogError("No action copied");
                return null;
            }

            return s_CurrentAction.Clone();
        }

        static public void PasteAction(RSActionData ioTarget)
        {
            if (!HasAction())
            {
                Debug.LogError("No action copied");
                return;
            }

            ioTarget.CopyFrom(s_CurrentAction);
        }

        #endregion // Action
    }
}