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
        [Serializable]
        private class ElementReferenceState
        {
            [NonSerialized] public IRSRuleTableSource Source;
            [NonSerialized] public RSRuleTableData Table;

            [SerializeField] public int RuleIndex = -1;
            public RSRuleData Rule { get; private set; }

            [SerializeField] public int ConditionIndex = -1;
            public RSConditionData Condition { get; private set; }

            [SerializeField] public int ActionIndex = -1;
            public RSActionData Action { get; private set; }

            public void Refresh()
            {
                if (Source == null)
                {
                    ClearSelections();
                    return;
                }

                if (Source.TableData == null)
                {
                    Table = null;
                    ClearSelections();
                    return;
                }

                Table = Source.TableData;
                Rule = Select(Table?.Rules, RuleIndex);
                Condition = Select(Rule?.Conditions, ConditionIndex);
                Action = Select(Rule?.Actions, ActionIndex);
            }

            static private T Select<T>(T[] inArray, int inIndex) where T : class
            {
                if (inIndex < 0 || inArray == null || inIndex >= inArray.Length)
                    return null;

                return inArray[inIndex];
            }

            public void ClearAll()
            {
                ClearSelections();
                Table = null;
            }

            public void ClearSelections()
            {
                RuleIndex = -1;
                ConditionIndex = -1;
                ActionIndex = -1;

                Rule = null;
                Condition = null;
                Action = null;
            }
        }

        [Serializable]
        private class TargetState
        {
            [SerializeField] public UnityEngine.Object SelectedObject;
            [NonSerialized] public UndoTarget UndoTarget;

            public void Refresh()
            {
                UndoTarget = SelectedObject;
            }

            public void Select(UnityEngine.Object inObject)
            {
                SelectedObject = inObject;
                Refresh();
            }

            public void Clear()
            {
                SelectedObject = null;
                Refresh();
            }
        }

        [Serializable]
        private class ScrollState
        {
            [SerializeField] public Vector2 TableScroll;
            [SerializeField] public Vector2 RuleScroll;
            [SerializeField] public Vector2 ElementInspectorScroll;
            [SerializeField] public Vector2 ErrorScroll;

            public void Reset()
            {
                TableScroll = RuleScroll = ElementInspectorScroll = ErrorScroll = Vector2.zero;
            }
        }

        private const float CLONE_BUTTON_WIDTH = 50;
        private const float CLONE_BUTTON_SPACING = 4;

        [NonSerialized] private UndoTarget m_SelfUndoTarget;

        [SerializeField] private TargetState m_TargetState = new TargetState();
        [SerializeField] private ScrollState m_ScrollState = new ScrollState();
        [SerializeField] private ElementReferenceState m_SelectionState = new ElementReferenceState();

        private RSValidationContext m_Context;

        static public bool Edit(UnityEngine.Object inTarget)
        {
            RuleTableEditor window = GetWindow<RuleTableEditor>();

            if (window.TryEdit(inTarget))
            {
                window.Show();
                return true;
            }

            return false;
        }

        static public bool Edit(IRSRuleTableSource inSource)
        {
            RuleTableEditor window = GetWindow<RuleTableEditor>();

            UnityEngine.Object objSource = inSource as UnityEngine.Object;
            if (objSource != null && window.TryEdit(objSource))
            {
                window.Show();
                return true;
            }

            return false;
        }

        static public bool Open(TableLineRef inRef)
        {
            RuleTableEditor window = GetWindow<RuleTableEditor>();

            if (window.TryOpen(inRef))
            {
                window.Show();
                return true;
            }

            return false;
        }

        #region Unity Events

        private void Awake()
        {
            m_TargetState.Refresh();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Rule Table Editor");
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            Undo.undoRedoPerformed += OnUndoRedoPerformed;

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            minSize = new Vector2(908, 400);
        }

        private void OnDisable()
        {
            Undo.undoRedoPerformed -= OnUndoRedoPerformed;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnGUI()
        {
            RegenContext();
            SyncAllowedListOperations();

            float width = this.position.width;
            float thirdWidth = Mathf.Max(width / 3, 300) - EditorStyles.helpBox.padding.left;

            using(new EditorGUILayout.HorizontalScope())
            {
                using(new EditorGUILayout.VerticalScope(GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true)))
                {
                    GUILayout.Space(4);

                    using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        RuleTableGUI();
                        GUILayout.FlexibleSpace();
                    }

                    using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        ErrorGUI();
                    }

                    using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        ConfigGUI();
                    }

                    GUILayout.Space(4);
                }

                EditorGUI.BeginChangeCheck();
                {
                    using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        RuleInfoGUI();
                    }

                    using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(thirdWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                    {
                        ConditionInfoGUI();
                        ActionInfoGUI();
                    }
                }
                if (EditorGUI.EndChangeCheck() && m_SelectionState.Rule != null && m_Context.Library != null)
                {
                    RuleFlags flags = TableUtils.GetRuleFlags(m_SelectionState.Rule, m_Context.Library);
                    if (flags != m_SelectionState.Rule.Flags)
                    {
                        m_TargetState.UndoTarget.MarkDirty("Modified rule flags");
                        m_SelectionState.Rule.Flags = flags;
                    }
                }
            }
        }

        private void OnUndoRedoPerformed()
        {
            ReapplyFromUndo();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            if (!m_TargetState.SelectedObject)
            {
                m_SelectionState.ClearAll();
                m_TargetState.Clear();
            }
        }

        private void SyncAllowedListOperations()
        {
            if (Event.current.type != EventType.Layout)
                return;

            SyncAllowedListOperations(m_RuleList);
            SyncAllowedListOperations(m_ActionList);
            SyncAllowedListOperations(m_ConditionList);
        }

        private void SyncAllowedListOperations(ReorderableList ioList)
        {
            if (ioList == null)
                return;

            bool bAllowArrayMod = !EditorApplication.isPlaying;
            ioList.displayAdd = ioList.displayRemove = bAllowArrayMod;
            ioList.draggable = bAllowArrayMod;
        }

        #endregion // Unity Events

        #region Internal

        private void RegenContext(bool inbForce = false)
        {
            if (!inbForce && Event.current.type != EventType.Layout)
                return;

            m_SelfUndoTarget = new UndoTarget(this, "RuleTableEditor");
            m_TargetState.Refresh();

            if (m_TargetState.SelectedObject == null)
            {
                m_TargetState.Clear();
                m_SelectionState.ClearAll();
                return;
            }

            if (m_TargetState.SelectedObject != null && m_SelectionState.Source == null)
            {
                IRSRuleTableSource tableSource;
                if (!RSEditorUtility.EditorPlugin.TryGetRuleTable(m_TargetState.SelectedObject, out tableSource))
                {
                    m_TargetState.Clear();
                    m_SelectionState.ClearAll();
                    return;
                }

                if (tableSource.TableData == null)
                {
                    m_TargetState.Clear();
                    m_SelectionState.ClearAll();
                    return;
                }

                m_SelectionState.Source = tableSource;
                m_SelectionState.Table = tableSource.TableData;

                bool bCleanUpChanged = TableUtils.CleanUp(tableSource.TableData);
                if (bCleanUpChanged)
                {
                    m_TargetState.UndoTarget.MarkDirtyWithoutUndo("Cleaned up Rule Table data");
                }
            }

            m_SelectionState.Refresh();

            m_Context = m_Context.WithLibrary(RSEditorUtility.EditorPlugin.Library);

            IRSEntity entity = null;
            if (m_TargetState.SelectedObject)
            {
                RSEditorUtility.EditorPlugin.TryGetEntity(m_TargetState.SelectedObject, out entity);
            }

            m_Context = m_Context.WithEntity(entity);

            IRSEntityMgr manager = null;
            if (manager == null || m_TargetState.SelectedObject)
            {
                RSEditorUtility.EditorPlugin.TryGetEntityManager(m_TargetState.SelectedObject, out manager);
            }

            m_Context = m_Context.WithManager(manager);
        }

        private void OnWillReorder(ReorderableList list)
        {
            m_TargetState.UndoTarget.MarkDirty("Reordering?", true);
        }

        private void ReapplyFromUndo()
        {
            if (m_SelectionState.Table == null)
            {
                StopEditing();
                return;
            }

            m_RuleList.array = m_SelectionState.Table.Rules;
            m_RuleList.index = m_SelectionState.RuleIndex;

            if (m_SelectionState.Rule != null)
            {
                m_ConditionList.array = m_SelectionState.Rule.Conditions;
                m_ConditionList.index = m_SelectionState.ConditionIndex;

                m_ActionList.array = m_SelectionState.Rule.Actions;
                m_ActionList.index = m_SelectionState.ActionIndex;
            }

            EditorApplication.delayCall += () => Repaint();
        }

        #endregion // Internal
    }
}