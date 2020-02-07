using System;
using BeauData;
using RuleScript.Data;
using RuleScript.Metadata;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace RuleScript.Editor
{
    public sealed partial class RuleTableEditor : EditorWindow
    {
        [NonSerialized] private RSReorderableList<RSConditionData> m_ConditionList;
        [NonSerialized] private RSReorderableList<RSActionData> m_ActionList;

        #region Rule Info

        private void SelectRule(int inIndex)
        {
            if (m_SelectionState.RuleIndex == inIndex)
                return;

            m_SelfUndoTarget.MarkDirty("Selected Rule");
            m_SelectionState.RuleIndex = inIndex;

            SelectAction(-1);
            SelectCondition(-1);

            if (inIndex < 0)
            {
                if (m_RuleList != null)
                    m_RuleList.index = -1;
                m_ConditionList = null;
                m_ActionList = null;
                return;
            }

            if (m_RuleList != null)
            {
                m_RuleList.index = inIndex;
            }
        }

        private void RuleInfoGUI()
        {
            if (m_SelectionState.Rule == null)
                return;

            if (m_SelectionState.Rule.Conditions == null)
            {
                m_TargetState.UndoTarget.MarkDirty("Initialized Conditions");
                m_SelectionState.Rule.Conditions = new RSConditionData[0];
            }

            if (m_SelectionState.Rule.Actions == null)
            {
                m_TargetState.UndoTarget.MarkDirty("Initialized Actions");
                m_SelectionState.Rule.Actions = new RSActionData[0];
            }

            RuleGUILayout.RuleData(m_TargetState.UndoTarget, m_SelectionState.Rule, m_SelectionState.Table, GetBaseFlags(), m_Context);

            EditorGUILayout.Separator();

            m_ScrollState.RuleScroll = GUILayout.BeginScrollView(m_ScrollState.RuleScroll, false, false);
            {
                EditorGUILayout.LabelField(Content.RuleConditionListLabel, RSGUIStyles.SubHeaderStyle);

                if (m_ConditionList == null)
                    ConfigureConditionList(m_SelectionState.Rule, ref m_ConditionList);

                m_ConditionList.array = m_SelectionState.Rule.Conditions;
                m_ConditionList.DoLayout();
                using(new EditorGUI.DisabledScope(m_SelectionState.Rule.Conditions.Length == 0))
                {
                    Subset currentSubset = m_SelectionState.Rule.ConditionSubset;
                    Subset nextSubset = (Subset) EditorGUILayout.EnumPopup(Content.RuleConditionSubsetLabel, currentSubset);
                    if (currentSubset != nextSubset)
                    {
                        m_TargetState.UndoTarget.MarkDirty("Changed Rule Conditions Subset");
                        m_SelectionState.Rule.ConditionSubset = nextSubset;
                    }
                }

                EditorGUILayout.Separator();

                EditorGUILayout.LabelField(Content.RuleActionListLabel, EditorStyles.boldLabel);

                if (m_ActionList == null)
                    ConfigureActionList(m_SelectionState.Rule, ref m_ActionList);

                m_ActionList.array = m_SelectionState.Rule.Actions;
                m_ActionList.DoLayout();
            }
            GUILayout.EndScrollView();
        }

        private RSTriggerInfo GetCurrentTrigger()
        {
            if (m_SelectionState.Rule == null)
                return null;

            return m_Context.Library.GetTrigger(m_SelectionState.Rule.TriggerId);
        }

        #region Reorderable Condition List

        private void ConfigureConditionList(RSRuleData inRule, ref RSReorderableList<RSConditionData> ioList)
        {
            ioList = new RSReorderableList<RSConditionData>(inRule.Conditions);
            ioList.drawElementCallback = RenderConditionListElement;
            ioList.drawNoneElementCallback = RenderNoConditionsElement;
            ioList.drawHeaderCallback = RenderConditionsHeaderElement;
            ioList.onAddCallback = OnAddNewCondition;
            ioList.onRemoveCallback = OnRemoveCondition;
            ioList.onSelectCallback = OnSelectCondition;
            ioList.onWillReorderCallback = OnWillReorder;
            ioList.onReorderCallback = OnConditionReorder;
            ioList.index = m_SelectionState.ConditionIndex;

            SyncAllowedListOperations(ioList);
        }

        private void RenderConditionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            RSConditionData condition = m_SelectionState.Rule.Conditions[index];

            Rect labelRect = rect;
            labelRect.width -= CLONE_BUTTON_WIDTH + CLONE_BUTTON_SPACING;

            string labelText = condition.GetPreviewString(GetCurrentTrigger(), m_Context.Library);

            using(new RSGUI.ColorScope(condition.Enabled ? Color.white : Color.gray))
            {
                EditorGUI.LabelField(labelRect, labelText);
            }

            Rect cloneRect = rect;
            cloneRect.width = CLONE_BUTTON_WIDTH;
            cloneRect.height -= 4;
            cloneRect.x = labelRect.xMax + CLONE_BUTTON_SPACING;

            using(new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                if (GUI.Button(cloneRect, "Clone"))
                {
                    RSConditionData clone = condition.Clone();
                    InsertCondition(clone, index + 1);
                }
            }

            if (DetectContextClick(rect))
            {
                ShowConditionElementContextMenu(condition, index);
            }
        }

        private void RenderNoConditionsElement(Rect rect)
        {
            EditorGUI.LabelField(rect, "No conditions");
        }

        private void RenderConditionsHeaderElement(Rect rect)
        {
            if (DetectContextClick(rect))
            {
                ShowConditionHeaderContextMenu();
            }
        }

        private void OnAddNewCondition(ReorderableList list)
        {
            RSConditionData conditionData = new RSConditionData();
            int index = list.index;
            if (index >= 0)
                ++index;
            InsertCondition(conditionData, index);
        }

        private void OnRemoveCondition(ReorderableList list)
        {
            int index = list.index;
            if (index >= 0)
                DeleteCondition(index);
        }

        private void OnSelectCondition(ReorderableList list)
        {
            SelectCondition(list.index);
        }

        private void OnConditionReorder(ReorderableList list)
        {
            m_SelfUndoTarget.MarkDirty("Reordered Condition");
            m_SelectionState.ConditionIndex = list.index;
        }

        private void InsertCondition(RSConditionData inCondition, int inIndex = -1)
        {
            m_TargetState.UndoTarget.MarkDirty("Added Condition", true);
            if (inIndex < 0 || inIndex >= m_SelectionState.Rule.Conditions.Length)
            {
                ArrayUtility.Add(ref m_SelectionState.Rule.Conditions, inCondition);
                inIndex = m_SelectionState.Rule.Conditions.Length - 1;
            }
            else
            {
                ArrayUtility.Insert(ref m_SelectionState.Rule.Conditions, inIndex, inCondition);
            }

            SelectCondition(inIndex);
        }

        private void DeleteCondition(int inIndex)
        {
            if (m_SelectionState.ConditionIndex == inIndex)
                SelectCondition(-1);
            else if (m_SelectionState.ConditionIndex > inIndex)
                SelectCondition(m_SelectionState.ConditionIndex - 1);

            m_TargetState.UndoTarget.MarkDirty("Removed Condition", true);
            ArrayUtility.RemoveAt(ref m_SelectionState.Rule.Conditions, inIndex);
        }

        private void ShowConditionElementContextMenu(RSConditionData inConditionData, int inIndex)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(s_ContextMenuCopyLabel, false, () => RSEditorClipboard.CopyCondition(inConditionData));
            if (RSEditorClipboard.HasCondition())
            {
                menu.AddItem(s_ContextMenuPasteOverwriteLabel, false, () =>
                {
                    m_TargetState.UndoTarget.MarkDirty("Paste condition (overwrite)");
                    RSEditorClipboard.PasteCondition(inConditionData);
                });
                if (EditorApplication.isPlaying)
                {
                    menu.AddDisabledItem(s_ContextMenuPasteInsertLabel, false);
                }
                else
                {
                    menu.AddItem(s_ContextMenuPasteInsertLabel, false, () =>
                    {
                        RSConditionData clone = RSEditorClipboard.PasteCondition();
                        InsertCondition(clone, inIndex + 1);
                    });
                }
            }
            else
            {
                menu.AddDisabledItem(s_ContextMenuPasteOverwriteLabel, false);
                menu.AddDisabledItem(s_ContextMenuPasteInsertLabel, false);
            }

            if (EditorApplication.isPlaying)
            {
                menu.AddDisabledItem(s_ContextMenuDeleteLabel, false);
            }
            else
            {
                menu.AddItem(s_ContextMenuDeleteLabel, false, () => DeleteCondition(inIndex));
            }

            menu.ShowAsContext();
        }

        private void ShowConditionHeaderContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            if (EditorApplication.isPlaying)
            {
                menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
            }
            else
            {
                if (RSEditorClipboard.HasCondition())
                {
                    menu.AddItem(s_ContextMenuPasteAddToEndLabel, false, () =>
                    {
                        RSConditionData clone = RSEditorClipboard.PasteCondition();
                        InsertCondition(clone, -1);
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                }

                if (m_SelectionState.Rule.Conditions.Length > 0)
                {
                    menu.AddItem(s_ContextMenuDeleteAllLabel, false, () =>
                    {
                        SelectCondition(-1);
                        m_TargetState.UndoTarget.MarkDirty("Removed all Conditions", true);
                        m_SelectionState.Rule.Conditions = new RSConditionData[0];
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
                }
            }

            menu.ShowAsContext();
        }

        #endregion // Reorderable Condition List

        #region Reorderable Action List

        private void ConfigureActionList(RSRuleData inRule, ref RSReorderableList<RSActionData> ioList)
        {
            ioList = new RSReorderableList<RSActionData>(inRule.Actions);
            ioList.drawElementCallback = RenderActionListElement;
            ioList.drawNoneElementCallback = RenderNoActionsElement;
            ioList.drawHeaderCallback = RenderActionsHeaderElement;
            ioList.onAddCallback = OnAddNewAction;
            ioList.onRemoveCallback = OnRemoveAction;
            ioList.onSelectCallback = OnSelectAction;
            ioList.onWillReorderCallback = OnWillReorder;
            ioList.onReorderCallback = OnActionReorder;
            ioList.index = m_SelectionState.ActionIndex;

            SyncAllowedListOperations(ioList);
        }

        private void RenderActionListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            RSActionData action = m_SelectionState.Rule.Actions[index];

            Rect labelRect = rect;
            labelRect.width -= CLONE_BUTTON_WIDTH + CLONE_BUTTON_SPACING;

            string labelText = action.GetPreviewString(GetCurrentTrigger(), m_Context.Library);

            using(new RSGUI.ColorScope(action.Enabled ? Color.white : Color.gray))
            {
                EditorGUI.LabelField(labelRect, labelText);
            }

            Rect cloneRect = rect;
            cloneRect.width = CLONE_BUTTON_WIDTH;
            cloneRect.height -= 4;
            cloneRect.x = labelRect.xMax + CLONE_BUTTON_SPACING;

            using(new EditorGUI.DisabledScope(EditorApplication.isPlaying))
            {
                if (GUI.Button(cloneRect, "Clone"))
                {
                    RSActionData clone = action.Clone();
                    InsertAction(clone, index + 1);
                }
            }

            if (DetectContextClick(rect))
            {
                ShowActionElementContextMenu(action, index);
            }
        }

        private void RenderNoActionsElement(Rect rect)
        {
            EditorGUI.LabelField(rect, "No actions");
        }

        private void RenderActionsHeaderElement(Rect rect)
        {
            if (DetectContextClick(rect))
            {
                ShowActionHeaderContextMenu();
            }
        }

        private void OnAddNewAction(ReorderableList list)
        {
            RSActionData actionData = new RSActionData();
            int index = list.index;
            if (index >= 0)
                ++index;
            InsertAction(actionData, index);
        }

        private void OnRemoveAction(ReorderableList list)
        {
            int index = list.index;
            if (index >= 0)
                DeleteAction(index);
        }

        private void OnSelectAction(ReorderableList list)
        {
            SelectAction(list.index);
        }

        private void OnActionReorder(ReorderableList list)
        {
            m_SelfUndoTarget.MarkDirty("Reordered Action");
            m_SelectionState.ActionIndex = list.index;
        }

        private void InsertAction(RSActionData inAction, int inIndex = -1)
        {
            m_TargetState.UndoTarget.MarkDirty("Added Action", true);
            if (inIndex < 0 || inIndex >= m_SelectionState.Rule.Actions.Length)
            {
                ArrayUtility.Add(ref m_SelectionState.Rule.Actions, inAction);
                inIndex = m_SelectionState.Rule.Actions.Length - 1;
            }
            else
            {
                ArrayUtility.Insert(ref m_SelectionState.Rule.Actions, inIndex, inAction);
            }

            SelectAction(inIndex);
        }

        private void DeleteAction(int inIndex)
        {
            if (m_SelectionState.ActionIndex == inIndex)
                SelectAction(-1);
            else if (m_SelectionState.ActionIndex > inIndex)
                SelectAction(m_SelectionState.ActionIndex - 1);

            m_TargetState.UndoTarget.MarkDirty("Removed Action", true);
            ArrayUtility.RemoveAt(ref m_SelectionState.Rule.Actions, inIndex);
        }

        private void ShowActionElementContextMenu(RSActionData inActionData, int inIndex)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(s_ContextMenuCopyLabel, false, () => RSEditorClipboard.CopyAction(inActionData));
            if (RSEditorClipboard.HasAction())
            {
                menu.AddItem(s_ContextMenuPasteOverwriteLabel, false, () =>
                {
                    m_TargetState.UndoTarget.MarkDirty("Paste action (overwrite)");
                    RSEditorClipboard.PasteAction(inActionData);
                });
                if (EditorApplication.isPlaying)
                {
                    menu.AddDisabledItem(s_ContextMenuPasteInsertLabel, false);
                }
                else
                {
                    menu.AddItem(s_ContextMenuPasteInsertLabel, false, () =>
                    {
                        RSActionData clone = RSEditorClipboard.PasteAction();
                        InsertAction(clone, inIndex + 1);
                    });
                }
            }
            else
            {
                menu.AddDisabledItem(s_ContextMenuPasteOverwriteLabel, false);
                menu.AddDisabledItem(s_ContextMenuPasteInsertLabel, false);
            }

            if (EditorApplication.isPlaying)
            {
                menu.AddDisabledItem(s_ContextMenuDeleteLabel, false);
            }
            else
            {
                menu.AddItem(s_ContextMenuDeleteLabel, false, () => DeleteAction(inIndex));
            }

            menu.ShowAsContext();
        }

        private void ShowActionHeaderContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            if (EditorApplication.isPlaying)
            {
                menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
            }
            else
            {
                if (RSEditorClipboard.HasAction())
                {
                    menu.AddItem(s_ContextMenuPasteAddToEndLabel, false, () =>
                    {
                        RSActionData clone = RSEditorClipboard.PasteAction();
                        InsertAction(clone, -1);
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                }

                if (m_SelectionState.Rule.Actions.Length > 0)
                {
                    menu.AddItem(s_ContextMenuDeleteAllLabel, false, () =>
                    {
                        SelectAction(-1);
                        m_TargetState.UndoTarget.MarkDirty("Removed all Actions", true);
                        m_SelectionState.Rule.Actions = new RSActionData[0];
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
                }
            }

            menu.ShowAsContext();
        }

        #endregion // Reorderable Action List

        #endregion // Rule Info
    }
}