using System;
using BeauData;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Validation;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RuleScript.Editor
{
    public sealed partial class RuleTableEditor : EditorWindow
    {
        #region Condition Info

        private void SelectCondition(int inIndex)
        {
            if (m_SelectionState.ConditionIndex == inIndex)
                return;

            m_SelfUndoTarget.MarkDirty("Selected Condition");
            m_SelectionState.ConditionIndex = inIndex;

            if (inIndex < 0)
            {
                if (m_ConditionList != null)
                    m_ConditionList.index = -1;
                return;
            }

            if (m_ConditionList != null)
            {
                m_ConditionList.index = inIndex;
            }

            SelectAction(-1);
        }

        private void ConditionInfoGUI()
        {
            if (m_SelectionState.Condition == null)
                return;

            m_ScrollState.ElementInspectorScroll = GUILayout.BeginScrollView(m_ScrollState.ElementInspectorScroll, false, false);
            {
                RSValidationContext context = m_Context.WithTrigger(GetCurrentTrigger());
                RuleGUILayout.ConditionData(m_TargetState.UndoTarget, m_SelectionState.Condition, GetBaseFlags(), context);
            }
            GUILayout.EndScrollView();
        }

        #endregion // Condition Info

        #region Action Info

        private void SelectAction(int inIndex)
        {
            if (m_SelectionState.ActionIndex == inIndex)
                return;

            m_SelfUndoTarget.MarkDirty("Selected Action");
            m_SelectionState.ActionIndex = inIndex;

            if (inIndex < 0)
            {
                if (m_ActionList != null)
                    m_ActionList.index = -1;
                return;
            }

            if (m_ActionList != null)
            {
                m_ActionList.index = inIndex;
            }

            SelectCondition(-1);
        }

        private void ActionInfoGUI()
        {
            if (m_SelectionState.Action == null)
                return;

            m_ScrollState.ElementInspectorScroll = GUILayout.BeginScrollView(m_ScrollState.ElementInspectorScroll, false, false);
            {
                RSValidationContext context = m_Context.WithTrigger(GetCurrentTrigger());
                RuleGUILayout.ActionData(m_TargetState.UndoTarget, m_SelectionState.Action, GetBaseFlags(), context);
            }
            GUILayout.EndScrollView();
        }

        #endregion // Action Info
    }
}