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
        [NonSerialized] private RSReorderableList<RSRuleData> m_RuleList;

        private bool TryEdit(UnityEngine.Object inObject)
        {
            IRSRuleTableSource ruleTableSource;
            bool bFoundTable = RSEditorUtility.EditorPlugin.TryGetRuleTable(inObject, out ruleTableSource);
            if (!bFoundTable || ruleTableSource.TableData == null)
                return false;

            m_TargetState.Select(inObject);

            m_SelectionState.ClearAll();
            m_SelectionState.Source = ruleTableSource;
            m_SelectionState.Table = ruleTableSource.TableData;

            if (m_SelectionState.Table.Rules == null)
            {
                m_TargetState.UndoTarget.MarkDirty("Initialized Rules");
                m_SelectionState.Table.Rules = new RSRuleData[0];
            }

            bool bCleanUpChanged = TableUtils.CleanUp(m_SelectionState.Table);
            if (bCleanUpChanged)
            {
                m_TargetState.UndoTarget.MarkDirtyWithoutUndo("Cleaned up Rule Table data");
            }

            ConfigureRuleList(m_SelectionState.Table, ref m_RuleList);

            m_Context = new RSValidationContext(RSEditorUtility.EditorPlugin.Library);
            RegenContext(true);
            ScanForIssues();

            UnityEditor.Selection.activeObject = inObject;
            return true;
        }

        private bool TryOpen(TableLineRef inRef)
        {
            UnityEngine.Object obj = inRef.TableSource as UnityEngine.Object;
            if (obj == null)
                return false;

            if (!TryEdit(obj))
                return false;

            int ruleIdx = TableUtils.IndexOfRule(m_SelectionState.Table, inRef.RuleId);
            if (ruleIdx >= 0)
            {
                SelectRule(ruleIdx);
                if (inRef.ConditionIndex >= 0)
                    SelectCondition(inRef.ConditionIndex);
                else if (inRef.ActionIndex >= 0)
                    SelectAction(inRef.ActionIndex);
            }

            m_SelectionState.Refresh();
            return true;
        }

        private void StopEditing()
        {
            m_SelectionState.ClearAll();
            m_TargetState.Clear();
            m_LastValidationState = null;

            m_RuleList = null;
            m_ActionList = null;
            m_ConditionList = null;

            m_Context = default(RSValidationContext);
        }

        private void RuleTableGUI()
        {
            if (m_SelectionState.Table == null)
            {
                EditorGUILayout.HelpBox("No table selected for editing", MessageType.Info);
                return;
            }

            if (!m_TargetState.UndoTarget.IsValid())
            {
                EditorGUILayout.HelpBox("Target is no longer valid (possibly destroyed?)", MessageType.Error);
                return;
            }

            string currentName = m_SelectionState.Table.Name;
            string nextName = EditorGUILayout.TextField(Content.TableNameLabel, currentName);
            if (currentName != nextName)
            {
                m_TargetState.UndoTarget.MarkDirty("Changed Rule Table Name");
                m_SelectionState.Table.Name = nextName;
            }

            EditorGUILayout.Separator();

            EditorGUILayout.LabelField(Content.TableRuleListLabel, RSGUIStyles.SubHeaderStyle);

            m_ScrollState.TableScroll = GUILayout.BeginScrollView(m_ScrollState.TableScroll, false, false);
            {
                if (m_RuleList == null)
                    ConfigureRuleList(m_SelectionState.Table, ref m_RuleList);

                m_RuleList.array = m_SelectionState.Table.Rules;
                m_RuleList.DoLayout();
            }
            GUILayout.EndScrollView();
        }

        #region Reorderable Rule List

        private void ConfigureRuleList(RSRuleTableData inTable, ref RSReorderableList<RSRuleData> ioList)
        {
            ioList = new RSReorderableList<RSRuleData>(inTable.Rules);
            ioList.drawElementCallback = RenderRuleListElement;
            ioList.drawNoneElementCallback = RenderNoRulesElement;
            ioList.drawHeaderCallback = RenderRulesHeaderElement;
            ioList.onAddCallback = OnAddNewRule;
            ioList.onRemoveCallback = OnRemoveRule;
            ioList.onSelectCallback = OnSelectRule;
            ioList.onWillReorderCallback = OnWillReorder;
            ioList.onReorderCallback = OnRuleReorder;
            ioList.index = m_SelectionState.RuleIndex;

            SyncAllowedListOperations(ioList);
        }

        private void RenderRuleListElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            RSRuleData rule = m_SelectionState.Table.Rules[index];

            Rect labelRect = rect;
            labelRect.width -= CLONE_BUTTON_WIDTH + CLONE_BUTTON_SPACING;

            string labelText = rule.GetPreviewString(null, m_Context.Library);

            using(new RSGUI.ColorScope(rule.Enabled ? Color.white : Color.gray))
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
                    RSRuleData clone = rule.Clone();
                    clone.Name += " (Clone)";
                    InsertRule(clone, index + 1);
                }
            }

            if (DetectContextClick(rect))
            {
                ShowRuleElementContextMenu(rule, index);
            }
        }

        private void RenderNoRulesElement(Rect rect)
        {
            EditorGUI.LabelField(rect, "No rules");
        }

        private void RenderRulesHeaderElement(Rect rect)
        {
            if (DetectContextClick(rect))
            {
                ShowRuleHeaderContextMenu();
            }
        }

        private void OnAddNewRule(ReorderableList list)
        {
            RSRuleData ruleData = new RSRuleData(true);
            int index = list.index;
            if (index >= 0)
                ++index;
            InsertRule(ruleData, index);
        }

        private void OnRemoveRule(ReorderableList list)
        {
            int index = list.index;
            if (index >= 0)
                DeleteRule(index);
        }

        private void OnSelectRule(ReorderableList list)
        {
            SelectRule(list.index);
        }

        private void OnRuleReorder(ReorderableList list)
        {
            m_SelfUndoTarget.MarkDirty("Reordered Rule");
            m_SelectionState.RuleIndex = list.index;
        }

        private void InsertRule(RSRuleData inRule, int inIndex = -1)
        {
            m_TargetState.UndoTarget.MarkDirty("Added Rule", true);
            if (inIndex < 0 || inIndex >= m_SelectionState.Table.Rules.Length)
            {
                ArrayUtility.Add(ref m_SelectionState.Table.Rules, inRule);
                inIndex = m_SelectionState.Table.Rules.Length - 1;
            }
            else
            {
                ArrayUtility.Insert(ref m_SelectionState.Table.Rules, inIndex, inRule);
            }

            TableUtils.UpdateUniqueRuleTriggers(m_SelectionState.Table);
            SelectRule(inIndex);
        }

        private void DeleteRule(int inIndex)
        {
            if (m_SelectionState.RuleIndex == inIndex)
                SelectRule(-1);
            else if (m_SelectionState.RuleIndex > inIndex)
                SelectRule(m_SelectionState.RuleIndex - 1);

            m_TargetState.UndoTarget.MarkDirty("Removed Rule", true);
            ArrayUtility.RemoveAt(ref m_SelectionState.Table.Rules, inIndex);
            TableUtils.UpdateUniqueRuleTriggers(m_SelectionState.Table);
        }

        private void ShowRuleElementContextMenu(RSRuleData inRuleData, int inIndex)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(s_ContextMenuCopyLabel, false, () => RSEditorClipboard.CopyRule(inRuleData));
            if (RSEditorClipboard.HasRule())
            {
                menu.AddItem(s_ContextMenuPasteOverwriteLabel, false, () =>
                {
                    m_TargetState.UndoTarget.MarkDirty("Paste rule (overwrite)");
                    RSEditorClipboard.PasteRule(inRuleData);
                });
                if (EditorApplication.isPlaying)
                {
                    menu.AddDisabledItem(s_ContextMenuPasteInsertLabel, false);
                }
                else
                {
                    menu.AddItem(s_ContextMenuPasteInsertLabel, false, () =>
                    {
                        RSRuleData clone = RSEditorClipboard.PasteRule();
                        InsertRule(clone, inIndex + 1);
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
                menu.AddItem(s_ContextMenuDeleteLabel, false, () => DeleteRule(inIndex));
            }

            menu.ShowAsContext();
        }

        private void ShowRuleHeaderContextMenu()
        {
            GenericMenu menu = new GenericMenu();
            if (EditorApplication.isPlaying)
            {
                menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
            }
            else
            {
                if (RSEditorClipboard.HasRule())
                {
                    menu.AddItem(s_ContextMenuPasteAddToEndLabel, false, () =>
                    {
                        RSRuleData clone = RSEditorClipboard.PasteRule();
                        InsertRule(clone, -1);
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuPasteAddToEndLabel, false);
                }

                if (m_SelectionState.Table.Rules.Length > 0)
                {
                    menu.AddItem(s_ContextMenuDeleteAllLabel, false, () =>
                    {
                        SelectRule(-1);
                        m_TargetState.UndoTarget.MarkDirty("Removed all Rules", true);
                        m_SelectionState.Table.Rules = new RSRuleData[0];
                    });
                }
                else
                {
                    menu.AddDisabledItem(s_ContextMenuDeleteAllLabel, false);
                }
            }

            menu.ShowAsContext();
        }

        #endregion // Reorderable Rule List
    }
}