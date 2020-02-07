using System;
using System.Collections.Generic;
using BeauData;
using BeauUtil;
using BeauUtil.Editor;
using RuleScript.Data;
using RuleScript.Metadata;
using RuleScript.Validation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuleScript.Editor
{
    public sealed partial class RefFinderEditor : EditorWindow
    {
        [LabeledEnum]
        private enum RootMode
        {
            [Label("RuleTable")]
            Table,

            [Label("Entity")]
            Entity
        }

        [LabeledEnum]
        private enum TableMode
        {
            [Label("Reference to Entity")]
            Entity,

            [Label("Reference to String")]
            String,

            [Label("Reference to Trigger")]
            Trigger,

            [Label("Reference to Action")]
            Action,

            [Label("Reference to Query")]
            Query,

            [Label("Has Issues")]
            Issues
        }

        [LabeledEnum]
        private enum EntityMode
        {
            [Label("Search by Name")]
            Name,

            [Label("Search by Prefab Name")]
            PrefabName,

            [Label("Search by RuleTable")]
            Table,

            [Label("Search by Link Ids")]
            LinkId,

            [Label("Search by Link Entity")]
            LinkEntity,

            [Label("Search By Entity Id")]
            Id
        }

        [Serializable]
        private class TableParams
        {
            [SerializeField] public TableMode Mode = TableMode.Trigger;

            [SerializeField] public string StringSearch = string.Empty;
            [SerializeField] public RSEntityId EntitySearch = RSEntityId.Null;
            [SerializeField] public int ElementSearchId = 0;

            public void Clear()
            {
                StringSearch = string.Empty;
                EntitySearch = RSEntityId.Null;
                ElementSearchId = 0;
            }
        }

        [Serializable]
        private class EntityParams
        {
            [SerializeField] public EntityMode Mode = EntityMode.Name;

            [SerializeField] public string StringSearch = string.Empty;
            [SerializeField] public int EntityIdSearch = 0;
            [SerializeField] public RSEntityId EntityRefSearch = RSEntityId.Null;
            [NonSerialized] public IRSRuleTableSource RuleTableSearch = null;

            public void Clear()
            {
                StringSearch = string.Empty;
                EntityIdSearch = 0;
                EntityRefSearch = RSEntityId.Null;
                RuleTableSearch = null;
            }
        }

        [Serializable]
        private class SearchParamsState
        {
            [SerializeField] public RootMode RootMode = RootMode.Table;
            [SerializeField] public TableParams TableParams = new TableParams();
            [SerializeField] public EntityParams EntityParams = new EntityParams();

            public void ClearAll()
            {
                TableParams.Clear();
                EntityParams.Clear();
            }
        }

        [Serializable]
        private class ScrollState
        {
            [SerializeField] public Vector2 RefScroll;

            public void Reset()
            {
                RefScroll = Vector2.zero;
            }
        }

        [NonSerialized] private UndoTarget m_SelfUndoTarget;
        [NonSerialized] private List<TableLineRef> m_LineRefs = new List<TableLineRef>();
        [NonSerialized] private List<EntityRef> m_EntityRefs = new List<EntityRef>();

        [NonSerialized] private IRSEntityMgr m_EntityMgr;
        [NonSerialized] private IRSRuleTableMgr m_TableMgr;
        [NonSerialized] private RSLibrary m_Library;

        [SerializeField] private SearchParamsState m_Params = new SearchParamsState();
        [SerializeField] private ScrollState m_ScrollState = new ScrollState();

        [NonSerialized] private bool m_SearchQueued = true;

        #region Static

        [MenuItem("Tools/RuleScript/Reference Finder")]
        static public void Open()
        {
            EditorWindow.GetWindow<RefFinderEditor>().Show();
        }

        #endregion // Static

        #region Unity Events

        private void OnEnable()
        {
            titleContent = new GUIContent("RS Ref Finder");

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            minSize = new Vector2(806, 400);
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnGUI()
        {
            RegenContext();

            float width = this.position.width;
            float halfWidth = width - 300 - EditorStyles.helpBox.padding.left;

            using(new EditorGUILayout.HorizontalScope())
            {
                using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(300), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
                {
                    SearchGUI();
                }

                using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox, GUILayout.Width(halfWidth), GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)))
                {
                    ResultsGUI();
                }
            }

            if (m_SearchQueued && Event.current.type == EventType.Repaint)
            {
                PerformSearch();
                Repaint();
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange stateChange)
        {
            m_LineRefs.Clear();
        }

        #endregion // Unity Events

        private void RegenContext()
        {
            Scene currentScene = EditorSceneManager.GetActiveScene();
            RSEditorUtility.EditorPlugin.TryGetEntityManager(currentScene, out m_EntityMgr);
            RSEditorUtility.EditorPlugin.TryGetRuleTableManager(currentScene, out m_TableMgr);
            m_Library = RSEditorUtility.EditorPlugin.Library;
        }

        #region Search GUI

        private void SearchGUI()
        {
            {
                RootMode nextMode = (RootMode) EnumGUILayout.EnumField("Mode", m_Params.RootMode);
                if (nextMode != m_Params.RootMode)
                {
                    m_SelfUndoTarget.MarkDirty("Switched mode");
                    m_Params.RootMode = nextMode;
                    m_SearchQueued = true;
                }
            }

            EditorGUILayout.Space();

            switch (m_Params.RootMode)
            {
                case RootMode.Entity:
                    EntityParamsGUI();
                    break;

                case RootMode.Table:
                    TableParamsGUI();
                    break;
            }

            GUILayout.FlexibleSpace();

            if (Event.current.isKey && Event.current.keyCode == KeyCode.F5)
            {
                m_SearchQueued = true;
            }

            if (GUILayout.Button("Perform Search (F5)"))
            {
                m_SearchQueued = true;
            }
        }

        private void EntityParamsGUI()
        {
            {
                EntityMode nextMode = (EntityMode) EnumGUILayout.EnumField("Search Type", m_Params.EntityParams.Mode);
                if (nextMode != m_Params.EntityParams.Mode)
                {
                    m_SelfUndoTarget.MarkDirty("Switched entity search mode");
                    m_Params.EntityParams.Mode = nextMode;
                    m_Params.EntityParams.Clear();
                    m_SearchQueued = true;
                }
            }

            switch (m_Params.EntityParams.Mode)
            {
                case EntityMode.Id:
                    {
                        int nextId = EditorGUILayout.IntField("Entity Id", m_Params.EntityParams.EntityIdSearch);
                        if (nextId != m_Params.EntityParams.EntityIdSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Switched entity id search");
                            m_Params.EntityParams.EntityIdSearch = nextId;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case EntityMode.LinkId:
                    {
                        string nextLink = EditorGUILayout.TextField("Link Id", m_Params.EntityParams.StringSearch);
                        if (nextLink != m_Params.EntityParams.StringSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Switched link search");
                            m_Params.EntityParams.StringSearch = nextLink;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case EntityMode.LinkEntity:
                    {
                        RSEntityId entityId = RSEditorUtility.EditorPlugin.EntityIdGUIField(s_EntityLabel, m_Params.EntityParams.EntityRefSearch, m_EntityMgr);
                        if (entityId != m_Params.EntityParams.EntityRefSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed entity search");
                            m_Params.EntityParams.EntityRefSearch = entityId;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case EntityMode.Name:
                    {
                        string nextName = EditorGUILayout.TextField("Name", m_Params.EntityParams.StringSearch);
                        if (nextName != m_Params.EntityParams.StringSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Switched name search");
                            m_Params.EntityParams.StringSearch = nextName;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case EntityMode.PrefabName:
                    {
                        string nextPrefab = EditorGUILayout.TextField("Prefab Name", m_Params.EntityParams.StringSearch);
                        if (nextPrefab != m_Params.EntityParams.StringSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Switched prefab name search");
                            m_Params.EntityParams.StringSearch = nextPrefab;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case EntityMode.Table:
                    {
                        EditorGUILayout.HelpBox("Not yet implemented", MessageType.Warning);
                        break;
                    }
            }
        }

        private void TableParamsGUI()
        {
            {
                TableMode nextMode = (TableMode) EnumGUILayout.EnumField("Search Type", m_Params.TableParams.Mode);
                if (nextMode != m_Params.TableParams.Mode)
                {
                    m_SelfUndoTarget.MarkDirty("Switched entity search mode");
                    m_Params.TableParams.Mode = nextMode;
                    m_Params.TableParams.Clear();
                    m_SearchQueued = true;
                }
            }

            switch (m_Params.TableParams.Mode)
            {
                case TableMode.Action:
                    {
                        int nextAction = LibraryGUILayout.ActionSelector(s_ActionLabel, m_Params.TableParams.ElementSearchId, m_Library);
                        if (nextAction != m_Params.TableParams.ElementSearchId)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed action id");
                            m_Params.TableParams.ElementSearchId = nextAction;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case TableMode.Query:
                    {
                        int nextQuery = LibraryGUILayout.QuerySelector(s_QueryLabel, m_Params.TableParams.ElementSearchId, false, m_Library);
                        if (nextQuery != m_Params.TableParams.ElementSearchId)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed query id");
                            m_Params.TableParams.ElementSearchId = nextQuery;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case TableMode.Trigger:
                    {
                        int nextTrigger = (int) LibraryGUILayout.TriggerSelector(s_TriggerLabel, new RSTriggerId(m_Params.TableParams.ElementSearchId), m_Library);
                        if (nextTrigger != m_Params.TableParams.ElementSearchId)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed trigger id");
                            m_Params.TableParams.ElementSearchId = nextTrigger;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case TableMode.Entity:
                    {
                        RSEntityId entityId = RSEditorUtility.EditorPlugin.EntityIdGUIField(s_EntityLabel, m_Params.TableParams.EntitySearch, m_EntityMgr);
                        if (entityId != m_Params.TableParams.EntitySearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed entity search");
                            m_Params.TableParams.EntitySearch = entityId;
                            m_SearchQueued = true;
                        }
                        break;
                    }

                case TableMode.String:
                    {
                        string nextString = EditorGUILayout.TextField("Search", m_Params.TableParams.StringSearch);
                        if (nextString != m_Params.TableParams.StringSearch)
                        {
                            m_SelfUndoTarget.MarkDirty("Changed string search");
                            m_Params.TableParams.StringSearch = nextString;
                            m_SearchQueued = true;
                        }
                        break;
                    }
            }
        }

        #endregion // Search GUI

        #region Results GUI

        private void ResultsGUI()
        {
            switch (m_Params.RootMode)
            {
                case RootMode.Entity:
                    EntityResultsGUI();
                    break;

                case RootMode.Table:
                    TableResultsGUI();
                    break;
            }
        }

        private void EntityResultsGUI()
        {
            if (m_EntityRefs.Count == 0)
            {
                GUILayout.Label("No Entity Results", RSGUIStyles.SubHeaderStyle);
                return;
            }

            GUILayout.Label(string.Format("{0} Entity Results", m_EntityRefs.Count), RSGUIStyles.SubHeaderStyle);

            EditorGUILayout.Space();

            m_ScrollState.RefScroll = GUILayout.BeginScrollView(m_ScrollState.RefScroll, false, false);
            {
                for (int i = 0; i < m_EntityRefs.Count; ++i)
                {
                    RenderEntityRef(m_EntityRefs[i], i);
                }
            }
            GUILayout.EndScrollView();
        }

        private void RenderEntityRef(EntityRef inEntityRef, int inIndex)
        {
            using(var scope = new EditorGUILayout.HorizontalScope())
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    Rect r = scope.rect;
                    if (r.Contains(Event.current.mousePosition))
                    {
                        var entity = m_EntityMgr.Lookup.EntityWithId(inEntityRef.Entity);
                        if (entity is UnityEngine.Object)
                        {
                            Selection.activeObject = (UnityEngine.Object) entity;
                        }
                    }
                }

                string prefix = string.Format("#{0}", inIndex + 1);
                using(new EditorGUI.DisabledScope(true))
                using(new GUIScopes.LabelWidthScope(50))
                {
                    RSEditorUtility.EditorPlugin.EntityIdGUIField(EditorGUIUtility.TrTempContent(prefix), inEntityRef.Entity, m_EntityMgr, GUILayout.Width(300));
                }

                if (!string.IsNullOrEmpty(inEntityRef.Descriptor))
                {
                    GUILayout.Label(inEntityRef.Descriptor);
                }
            }
        }

        private void TableResultsGUI()
        {
            if (m_LineRefs.Count == 0)
            {
                GUILayout.Label("No Table Results", RSGUIStyles.SubHeaderStyle);
                return;
            }

            GUILayout.Label(string.Format("{0} Table Results", m_LineRefs.Count), RSGUIStyles.SubHeaderStyle);

            EditorGUILayout.Space();

            m_ScrollState.RefScroll = GUILayout.BeginScrollView(m_ScrollState.RefScroll, false, false);
            {
                for (int i = 0; i < m_LineRefs.Count; ++i)
                {
                    RenderLineRef(m_LineRefs[i], i);
                }
            }
            GUILayout.EndScrollView();
        }

        private void RenderLineRef(TableLineRef inLineRef, int inIndex)
        {
            using(var scope = new EditorGUILayout.HorizontalScope())
            using(new GUIScopes.ColorScope(inLineRef.Enabled ? Color.white : Color.gray))
            {
                if (Event.current.type == EventType.MouseDown)
                {
                    Rect r = scope.rect;
                    if (r.Contains(Event.current.mousePosition))
                    {
                        RuleTableEditor.Open(inLineRef);
                    }
                }

                string prefix = string.Format("#{0}", inIndex + 1);
                using(new EditorGUI.DisabledScope(true))
                using(new GUIScopes.LabelWidthScope(50))
                {
                    RSEditorUtility.EditorPlugin.RuleTableSourceField(EditorGUIUtility.TrTempContent(prefix), inLineRef.TableSource, m_TableMgr, GUILayout.Width(300));
                }

                string ruleName = "[Unknown Rule]";
                if (!string.IsNullOrEmpty(inLineRef.RuleId) && inLineRef.TableSource != null)
                {
                    int ruleIdx = TableUtils.IndexOfRule(inLineRef.TableSource.TableData, inLineRef.RuleId);
                    if (ruleIdx >= 0)
                        ruleName = inLineRef.TableSource.TableData.Rules[ruleIdx].Name;
                }

                using(new GUIScopes.LabelWidthScope(400))
                {
                    if (inLineRef.ActionIndex >= 0)
                    {
                        string actionLabel = string.Format("Rule '{0}' -> Action {1}", ruleName, inLineRef.ActionIndex);
                        EditorGUILayout.LabelField(actionLabel);
                    }
                    else if (inLineRef.ConditionIndex >= 0)
                    {
                        string conditionLabel = string.Format("Rule '{0}' -> Condition {1}", ruleName, inLineRef.ConditionIndex);
                        EditorGUILayout.LabelField(conditionLabel);
                    }
                    else if (!string.IsNullOrEmpty(inLineRef.RuleId))
                    {
                        string ruleLabel = string.Format("Rule '{0}'", ruleName);
                        EditorGUILayout.LabelField(ruleLabel);
                    }
                }

                GUILayout.FlexibleSpace();

                if (!string.IsNullOrEmpty(inLineRef.Descriptor))
                {
                    GUILayout.Label(inLineRef.Descriptor);
                }

                if (GUILayout.Button("...", GUILayout.Width(25)))
                {
                    RuleTableEditor.Open(inLineRef);
                }
            }
        }

        #endregion // Results

        #region Searches

        private void PerformSearch()
        {
            m_SearchQueued = false;

            switch (m_Params.RootMode)
            {
                case RootMode.Entity:
                    PerformEntitySearch(m_Params.EntityParams);
                    break;

                case RootMode.Table:
                    PerformTableSearch(m_Params.TableParams);
                    break;
            }
        }

        private void PerformEntitySearch(EntityParams inEntityParams)
        {
            m_EntityRefs.Clear();

            if (m_EntityMgr == null)
            {
                Debug.LogError("No entity manager found");
                return;
            }

            switch (inEntityParams.Mode)
            {
                case EntityMode.Id:
                    {
                        foreach (var entity in m_EntityMgr.Lookup.AllEntities())
                        {
                            if ((int) entity.Id == inEntityParams.EntityIdSearch)
                            {
                                m_EntityRefs.Add(EntityRef.FromEntity(entity).WithDescriptor(inEntityParams.EntityIdSearch.ToString()));
                            }
                        }
                        break;
                    }

                case EntityMode.LinkId:
                    {
                        foreach (var entity in m_EntityMgr.Lookup.AllEntities())
                        {
                            foreach (var link in entity.Links.AllLinks())
                            {
                                if (StringMatch(link.Value, inEntityParams.StringSearch))
                                {
                                    m_EntityRefs.Add(EntityRef.FromEntity(entity).WithDescriptor(link.Value));
                                }
                            }
                        }
                        break;
                    }

                case EntityMode.LinkEntity:
                    {
                        foreach (var entity in m_EntityMgr.Lookup.AllEntities())
                        {
                            foreach (var link in entity.Links.AllLinks())
                            {
                                if (link.Key == inEntityParams.EntityRefSearch)
                                {
                                    m_EntityRefs.Add(EntityRef.FromEntity(entity).WithDescriptor(link.Value));
                                }
                            }
                        }
                        break;
                    }

                case EntityMode.Name:
                    {
                        foreach (var entity in m_EntityMgr.Lookup.AllEntities())
                        {
                            if (StringMatch(entity.Name, inEntityParams.StringSearch))
                            {
                                m_EntityRefs.Add(EntityRef.FromEntity(entity).WithDescriptor(entity.Name));
                            }
                        }
                        break;
                    }

                case EntityMode.PrefabName:
                    {
                        foreach (var entity in m_EntityMgr.Lookup.AllEntities())
                        {
                            if (StringMatch(entity.Prefab, inEntityParams.StringSearch))
                            {
                                m_EntityRefs.Add(EntityRef.FromEntity(entity).WithDescriptor(entity.Prefab));
                            }
                        }
                        break;
                    }

                case EntityMode.Table:
                    {
                        // TODO(Beau): Implement
                        break;
                    }
            }
        }

        private void PerformTableSearch(TableParams inTableParams)
        {
            m_LineRefs.Clear();

            if (m_TableMgr == null)
            {
                Debug.LogError("No rule table manager found");
                return;
            }

            AbstractTableRefVisitor visitor = null;

            switch (inTableParams.Mode)
            {
                case TableMode.Action:
                    {
                        visitor = new TableUtils.ActionIdRefVisitor(inTableParams.ElementSearchId);
                        break;
                    }

                case TableMode.Query:
                    {
                        visitor = new TableUtils.QueryIdRefVisitor(inTableParams.ElementSearchId);
                        break;
                    }

                case TableMode.Trigger:
                    {
                        visitor = new TableUtils.TriggerIdRefVisitor(new RSTriggerId(inTableParams.ElementSearchId));
                        break;
                    }

                case TableMode.Entity:
                    {
                        visitor = new TableUtils.EntityIdRefVisitor(inTableParams.EntitySearch);
                        break;
                    }

                case TableMode.String:
                    {
                        visitor = new TableUtils.StringRefVisitor(inTableParams.StringSearch);
                        break;
                    }

                case TableMode.Issues:
                    {
                        var validationContext = new RSValidationContext(RSEditorUtility.EditorPlugin.Library);
                        validationContext = validationContext.WithManager(m_EntityMgr);
                        foreach (var table in m_TableMgr.AllTableSources())
                        {
                            var validationState = RSValidator.Validate(table.TableData, validationContext);
                            if (validationState.IssueCount > 0)
                            {
                                m_LineRefs.Add(TableLineRef.FromTable(table).WithDescriptor("{0} errors, {1} warnings", validationState.ErrorCount, validationState.WarningCount));
                            }
                        }
                        break;
                    }
            }

            if (visitor != null)
            {
                foreach (var table in m_TableMgr.AllTableSources())
                {
                    visitor.Visit(table);
                }
                m_LineRefs.AddRange(visitor.CollectedRefs);
            }
        }

        static private bool StringMatch(string inString, string inMatch)
        {
            if (string.IsNullOrEmpty(inString))
                return false;

            return inString.IndexOf(inMatch, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion // Searches

        #region Content

        static private readonly GUIContent s_ActionLabel = new GUIContent("Action");
        static private readonly GUIContent s_QueryLabel = new GUIContent("Query");
        static private readonly GUIContent s_TriggerLabel = new GUIContent("Trigger");
        static private readonly GUIContent s_EntityLabel = new GUIContent("Entity");

        #endregion // Content
    }
}