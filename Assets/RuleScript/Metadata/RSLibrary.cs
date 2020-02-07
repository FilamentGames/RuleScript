using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BeauData;
using BeauUtil;
using RuleScript.Data;
using RuleScript.Runtime;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Library of info on rule scripting elements.
    /// </summary>
    public sealed class RSLibrary
    {
        private readonly Dictionary<int, RSComponentInfo> m_Components = new Dictionary<int, RSComponentInfo>();
        private readonly Dictionary<Type, RSComponentInfo> m_ComponentTypeMap = new Dictionary<Type, RSComponentInfo>();
        private readonly Dictionary<int, RSTriggerInfo> m_Triggers = new Dictionary<int, RSTriggerInfo>();
        private readonly Dictionary<int, RSQueryInfo> m_Queries = new Dictionary<int, RSQueryInfo>();
        private readonly Dictionary<int, RSActionInfo> m_Actions = new Dictionary<int, RSActionInfo>();
        private readonly Dictionary<int, RSGroupInfo> m_Groups = new Dictionary<int, RSGroupInfo>();

        internal readonly RSTypeAssembly TypeAssembly;
        private bool m_IsLoading = false;
        private bool m_IsLoaded = false;
        private readonly Type m_EntityType;

        public RSLibrary() : this(typeof(IRSRuntimeEntity)) { }

        public RSLibrary(Type inEntityType)
        {
            TypeAssembly = new RSTypeAssembly(RSBuiltInTypes.BaseAssembly);
            m_EntityType = inEntityType;
        }

        #region Construction

        public bool IsLoading()
        {
            return m_IsLoading;
        }

        public bool IsLoaded()
        {
            return m_IsLoaded;
        }

        public void ScanAssemblies(Action<float, string> inOnProgress = null)
        {
            if (m_IsLoaded)
                return;

            var asyncVersion = ScanAssembliesAsync(inOnProgress);
            while (asyncVersion.MoveNext());
        }

        public IEnumerator ScanAssembliesAsync(Action<float, string> inOnProgress = null)
        {
            if (m_IsLoaded)
                yield break;

            m_IsLoading = true;
            m_IsLoaded = false;

            List<Assembly> allowedAssemblies = new List<Assembly>(GetAssembliesToScan());
            List<RSInfo> toLink = new List<RSInfo>();

            inOnProgress?.Invoke(0, "Loading groups");

            foreach (var groupInfo in Reflect.FindFields<RSGroupAttribute>(allowedAssemblies, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                RSGroupInfo groupMeta = new RSGroupInfo(groupInfo.Attribute, groupInfo.Info);
                m_Groups.Add(groupMeta.IdHash, groupMeta);
                groupInfo.Info.SetValue(null, groupMeta.GroupId);
                yield return null;
            }

            inOnProgress?.Invoke(1f / 6, "Loading components");

            foreach (var componentInfo in Reflect.FindTypes<RSComponentAttribute>(allowedAssemblies))
            {
                RSComponentInfo componentMeta = new RSComponentInfo(componentInfo.Attribute, componentInfo.Info);
                m_Components.Add(componentMeta.IdHash, componentMeta);
                m_ComponentTypeMap.Add(componentInfo.Info, componentMeta);
                toLink.Add(componentMeta);
                yield return null;
            }

            inOnProgress?.Invoke(2f / 6, "Loading queries");

            foreach (var queryInfo in Reflect.FindMethods<RSQueryAttribute>(allowedAssemblies, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                RSQueryInfo queryMeta = new RSQueryInfo(queryInfo.Attribute, queryInfo.Info);
                m_Queries.Add(queryMeta.IdHash, queryMeta);
                toLink.Add(queryMeta);
                yield return null;
            }

            inOnProgress?.Invoke(3f / 6, "Loading actions");

            foreach (var actionInfo in Reflect.FindMethods<RSActionAttribute>(allowedAssemblies, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                RSActionInfo actionMeta = new RSActionInfo(actionInfo.Attribute, actionInfo.Info);
                m_Actions.Add(actionMeta.IdHash, actionMeta);
                toLink.Add(actionMeta);
                yield return null;
            }

            inOnProgress?.Invoke(4f / 6, "Loading triggers");

            foreach (var triggerInfo in Reflect.FindFields<RSTriggerAttribute>(allowedAssemblies, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                RSTriggerInfo triggerMeta = new RSTriggerInfo(triggerInfo.Attribute, triggerInfo.Info);
                m_Triggers.Add(triggerMeta.IdHash, triggerMeta);
                triggerInfo.Info.SetValue(null, triggerMeta.TriggerId);
                toLink.Add(triggerMeta);
                yield return null;
            }

            inOnProgress?.Invoke(5f / 6, "Linking elements");

            foreach (var meta in toLink)
            {
                meta.Link(TypeAssembly);
                yield return null;
            }

            toLink.Clear();

            m_IsLoading = false;
            m_IsLoaded = true;

            inOnProgress?.Invoke(1, "Done");
        }

        #endregion // Construction

        #region Components

        /// <summary>
        /// Finds the component metadata with the given id. 
        /// </summary>
        public RSComponentInfo GetComponent(string inId)
        {
            if (string.IsNullOrEmpty(inId))
                return null;

            RSComponentInfo meta;
            if (!m_Components.TryGetValue(ScriptUtils.Hash(inId), out meta))
            {
                Log.Error("[RSMetaDatabase] No component type with id '{0}' registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the component metadata with the given id. 
        /// </summary>
        public RSComponentInfo GetComponent(int inId)
        {
            if (inId == 0)
                return null;

            RSComponentInfo meta;
            if (!m_Components.TryGetValue(inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No component type with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the component metadata associated with the given type. 
        /// </summary>
        public RSComponentInfo GetComponent(Type inType)
        {
            if (inType == null)
                return null;

            RSComponentInfo meta;
            if (!m_ComponentTypeMap.TryGetValue(inType, out meta))
            {
                Log.Error("[RSMetaDatabase] No component type with type {0} registered", inType);
            }
            return meta;
        }

        /// <summary>
        /// Gets a collection of all compoment metadatas.
        /// </summary>
        public IReadOnlyCollection<RSComponentInfo> GetAllComponents()
        {
            return m_Components.Values;
        }

        #endregion // Components

        #region Triggers

        /// <summary>
        /// Finds the trigger metadata with the given id. 
        /// </summary>
        public RSTriggerInfo GetTrigger(string inId)
        {
            if (string.IsNullOrEmpty(inId))
                return null;

            RSTriggerInfo meta;
            if (!m_Triggers.TryGetValue(ScriptUtils.Hash(inId), out meta))
            {
                Log.Error("[RSMetaDatabase] No trigger type with id '{0}' registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the trigger metadata with the given id. 
        /// </summary>
        public RSTriggerInfo GetTrigger(RSTriggerId inId)
        {
            if (inId == RSTriggerId.Null)
                return null;

            RSTriggerInfo meta;
            if (!m_Triggers.TryGetValue((int) inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No trigger type with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the trigger metadata with the given id. 
        /// </summary>
        public RSTriggerInfo GetTrigger(int inId)
        {
            if (inId == 0)
                return null;

            RSTriggerInfo meta;
            if (!m_Triggers.TryGetValue(inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No trigger type with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Gets a collection of all trigger metadatas.
        /// </summary>
        public IReadOnlyCollection<RSTriggerInfo> GetAllTriggers()
        {
            return m_Triggers.Values;
        }

        /// <summary>
        /// Outputs to a collection of all trigger metadatas.
        /// </summary>
        public void GetAllTriggers(ICollection<RSTriggerInfo> outTriggers, RSTypeInfo inParameterType = null)
        {
            Assert.True(outTriggers != null, "Cannot output triggers to null collection");

            foreach (var triggerMeta in m_Triggers.Values)
            {
                if (!MatchTriggerParameter(triggerMeta, inParameterType))
                    continue;

                outTriggers.Add(triggerMeta);
            }
        }

        /// <summary>
        /// Outputs to a collection of triggers associated with the global scope.
        /// </summary>
        public void GetAllGlobalTriggers(ICollection<RSTriggerInfo> outTriggers, RSTypeInfo inParameterType = null)
        {
            Assert.True(outTriggers != null, "Cannot output triggers to null collection");

            foreach (var triggerMeta in m_Triggers.Values)
            {
                Type type = triggerMeta.OwnerType;
                if (type == null)
                {
                    if (!MatchTriggerParameter(triggerMeta, inParameterType))
                        continue;

                    outTriggers.Add(triggerMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of triggers associated with a local scope.
        /// </summary>
        public void GetAllLocalTriggers(ICollection<RSTriggerInfo> outTriggers, RSTypeInfo inParameterType = null)
        {
            Assert.True(outTriggers != null, "Cannot output triggers to null collection");

            foreach (var triggerMeta in m_Triggers.Values)
            {
                Type type = triggerMeta.OwnerType;
                if (type != null)
                {
                    if (!MatchTriggerParameter(triggerMeta, inParameterType))
                        continue;

                    outTriggers.Add(triggerMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of triggers associated with the given type.
        /// </summary>
        public void GetAllTriggersForType(Type inType, ICollection<RSTriggerInfo> outTriggers, RSTypeInfo inParameterType = null)
        {
            Assert.True(outTriggers != null, "Cannot output triggers to null collection");
            Assert.True(inType != null, "Cannot get triggers for null type");

            foreach (var triggerMeta in m_Triggers.Values)
            {
                Type type = triggerMeta.OwnerType;
                if (type != null && type.IsAssignableFrom(inType))
                {
                    if (!MatchTriggerParameter(triggerMeta, inParameterType))
                        continue;

                    outTriggers.Add(triggerMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of triggers directly associated with the given entity.
        /// </summary>
        public void GetAllTriggersForEntity(IRSEntity inEntity, ICollection<RSTriggerInfo> outTriggers, RSTypeInfo inParameterType = null)
        {
            Assert.True(outTriggers != null, "Cannot output triggers to null collection");
            Assert.True(inEntity != null, "Cannot get triggers for null entity data");

            GetAllTriggersForType(m_EntityType, outTriggers, inParameterType);
            foreach (var componentType in inEntity.GetRSComponentTypes(this))
            {
                Assert.True(componentType != null, "Null component data");
                GetAllTriggersForType(componentType, outTriggers, inParameterType);
            }
        }

        static private bool MatchTriggerParameter(RSTriggerInfo inInfo, RSTypeInfo inTypeInfo)
        {
            if (inTypeInfo == null)
                return true;

            if (inInfo.ParameterType == null)
                return inTypeInfo == RSBuiltInTypes.Void;

            return inTypeInfo.CanConvert(inInfo.ParameterType.Type);
        }

        #endregion // Triggers

        #region Queries

        /// <summary>
        /// Finds the query metadata with the given id.
        /// </summary>
        public RSQueryInfo GetQuery(string inId)
        {
            if (string.IsNullOrEmpty(inId))
                return null;

            RSQueryInfo meta;
            if (!m_Queries.TryGetValue(ScriptUtils.Hash(inId), out meta))
            {
                Log.Error("[RSMetaDatabase] No query type with id '{0}' registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the query metadata with the given id. 
        /// </summary>
        public RSQueryInfo GetQuery(int inId)
        {
            if (inId == 0)
                return null;

            RSQueryInfo meta;
            if (!m_Queries.TryGetValue(inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No query type with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Gets a collection of all query metadatas.
        /// </summary>
        public IReadOnlyCollection<RSQueryInfo> GetAllQueries()
        {
            return m_Queries.Values;
        }

        /// <summary>
        /// Outputs to a collection of all query metadatas.
        /// </summary>
        public void GetAllQueries(ICollection<RSQueryInfo> outQueries, RSTypeInfo inReturnType = null, bool inbNoParams = false)
        {
            Assert.True(outQueries != null, "Cannot output queries to null collection");

            foreach (var queryMeta in m_Queries.Values)
            {
                if (inReturnType != null && !queryMeta.ReturnType.CanConvert(inReturnType))
                    continue;

                if (inbNoParams && queryMeta.Parameters != null && queryMeta.Parameters.Length > 0)
                    continue;

                outQueries.Add(queryMeta);
            }
        }

        /// <summary>
        /// Outputs to a collection of queries associated with the global scope.
        /// </summary>
        public void GetAllGlobalQueries(ICollection<RSQueryInfo> outQueries, RSTypeInfo inReturnType = null, bool inbNoParams = false)
        {
            Assert.True(outQueries != null, "Cannot output queries to null collection");

            foreach (var queryMeta in m_Queries.Values)
            {
                Type type = queryMeta.OwnerType;
                if (type == null)
                {
                    if (inReturnType != null && !queryMeta.ReturnType.CanConvert(inReturnType))
                        continue;

                    if (inbNoParams && queryMeta.Parameters != null && queryMeta.Parameters.Length > 0)
                        continue;

                    outQueries.Add(queryMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of queries associated with a local scope.
        /// </summary>
        public void GetAllLocalQueries(ICollection<RSQueryInfo> outQueries, RSTypeInfo inReturnType = null, bool inbNoParams = false)
        {
            Assert.True(outQueries != null, "Cannot output queries to null collection");

            foreach (var queryMeta in m_Queries.Values)
            {
                Type type = queryMeta.OwnerType;
                if (type != null)
                {
                    if (inReturnType != null && !queryMeta.ReturnType.CanConvert(inReturnType))
                        continue;

                    if (inbNoParams && queryMeta.Parameters != null && queryMeta.Parameters.Length > 0)
                        continue;

                    outQueries.Add(queryMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of queries associated with the given type.
        /// </summary>
        public void GetAllQueriesForType(Type inType, ICollection<RSQueryInfo> outQueries, RSTypeInfo inReturnType = null, bool inbNoParams = false)
        {
            Assert.True(outQueries != null, "Cannot output queries to null collection");
            Assert.True(inType != null, "Cannot get queries for null type");

            foreach (var queryMeta in m_Queries.Values)
            {
                Type type = queryMeta.OwnerType;
                if (type != null && type.IsAssignableFrom(inType))
                {
                    if (inReturnType != null && !queryMeta.ReturnType.CanConvert(inReturnType))
                        continue;

                    if (inbNoParams && queryMeta.Parameters != null && queryMeta.Parameters.Length > 0)
                        continue;

                    outQueries.Add(queryMeta);
                }
            }
        }

        /// <summary>
        /// Outputs to a collection of queries directly associated with the given entity.
        /// </summary>
        public void GetAllQueriesForEntity(IRSEntity inEntity, ICollection<RSQueryInfo> outQueries, RSTypeInfo inReturnType = null, bool inbNoParams = false)
        {
            Assert.True(outQueries != null, "Cannot output queries to null collection");
            Assert.True(inEntity != null, "Cannot get queries for null entity");

            GetAllQueriesForType(m_EntityType, outQueries, inReturnType, inbNoParams);
            foreach (var component in inEntity.GetRSComponentTypes(this))
            {
                Assert.True(component != null, "Null component");
                GetAllQueriesForType(component, outQueries, inReturnType, inbNoParams);
            }
        }

        #endregion // Queries

        #region Actions

        /// <summary>
        /// Finds the action metadata with the given id. 
        /// </summary>
        public RSActionInfo GetAction(string inId)
        {
            if (string.IsNullOrEmpty(inId))
                return null;

            RSActionInfo meta;
            if (!m_Actions.TryGetValue(ScriptUtils.Hash(inId), out meta))
            {
                Log.Error("[RSMetaDatabase] No action type with id '{0}' registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the action metadata with the given id. 
        /// </summary>
        public RSActionInfo GetAction(int inId)
        {
            if (inId == 0)
                return null;

            RSActionInfo meta;
            if (!m_Actions.TryGetValue(inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No action type with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Gets a collection of all action metadatas.
        /// </summary>
        public IReadOnlyCollection<RSActionInfo> GetAllActions()
        {
            return m_Actions.Values;
        }

        /// <summary>
        /// Outputs to a collection of all action metadatas.
        /// </summary>
        public void GetAllActions(ICollection<RSActionInfo> outActions)
        {
            Assert.True(outActions != null, "Cannot output actions to null collection");

            foreach (var actionMeta in m_Actions.Values)
                outActions.Add(actionMeta);
        }

        /// <summary>
        /// Outputs to a collection of actions associated with the global scope.
        /// </summary>
        public void GetAllGlobalActions(ICollection<RSActionInfo> outActions)
        {
            Assert.True(outActions != null, "Cannot output actions to null collection");

            foreach (var actionMeta in m_Actions.Values)
            {
                Type type = actionMeta.OwnerType;
                if (type == null)
                    outActions.Add(actionMeta);
            }
        }

        /// <summary>
        /// Outputs to a collection of actions associated with a local scope.
        /// </summary>
        public void GetAllLocalActions(ICollection<RSActionInfo> outActions)
        {
            Assert.True(outActions != null, "Cannot output actions to null collection");

            foreach (var actionMeta in m_Actions.Values)
            {
                Type type = actionMeta.OwnerType;
                if (type != null)
                    outActions.Add(actionMeta);
            }
        }

        /// <summary>
        /// Outputs to a collection of actions associated with the given type.
        /// </summary>
        public void GetAllActionsForType(Type inType, ICollection<RSActionInfo> outActions)
        {
            Assert.True(outActions != null, "Cannot output actions to null collection");
            Assert.True(inType != null, "Cannot get actions for null type");

            foreach (var actionMeta in m_Actions.Values)
            {
                Type type = actionMeta.OwnerType;
                if (type != null && type.IsAssignableFrom(inType))
                    outActions.Add(actionMeta);
            }
        }

        /// <summary>
        /// Outputs to a collection of actions directly associated with the given entity.
        /// </summary>
        public void GetAllActionsForEntity(IRSEntity inEntity, ICollection<RSActionInfo> outActions)
        {
            Assert.True(outActions != null, "Cannot output actions to null collection");
            Assert.True(inEntity != null, "Cannot get actions for null entity");

            GetAllActionsForType(m_EntityType, outActions);

            foreach (var component in inEntity.GetRSComponentTypes(this))
            {
                Assert.True(component != null, "Null component");
                GetAllActionsForType(component, outActions);
            }
        }

        #endregion // Actions

        #region Groups

        /// <summary>
        /// Finds the group metadata associated with the given id.
        /// </summary>
        public RSGroupInfo GetGroup(string inId)
        {
            if (string.IsNullOrEmpty(inId))
                return null;

            RSGroupInfo meta;
            if (!m_Groups.TryGetValue(ScriptUtils.Hash(inId), out meta))
            {
                Log.Error("[RSMetaDatabase] No group with id '{0}' registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the group metadata associated with the given id.
        /// </summary>
        public RSGroupInfo GetGroup(RSGroupId inId)
        {
            if (inId == RSGroupId.Null)
                return null;

            RSGroupInfo meta;
            if (!m_Groups.TryGetValue((int) inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No group with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Finds the group metadata associated with the given id.
        /// </summary>
        public RSGroupInfo GetGroup(int inId)
        {
            if (inId == 0)
                return null;

            RSGroupInfo meta;
            if (!m_Groups.TryGetValue(inId, out meta))
            {
                Log.Error("[RSMetaDatabase] No group with id {0} registered", inId);
            }
            return meta;
        }

        /// <summary>
        /// Gets a collection of all group metadatas.
        /// </summary>
        public IReadOnlyCollection<RSGroupInfo> GetAllGroups()
        {
            return m_Groups.Values;
        }

        /// <summary>
        /// Gets a collection of all group metadatas.
        /// </summary>
        public void GetAllGroups(ICollection<RSGroupInfo> outGroups)
        {
            Assert.True(outGroups != null, "Cannot output groups to null collection");

            foreach (var groupMeta in m_Groups.Values)
                outGroups.Add(groupMeta);
        }

        #endregion // Groups

        #region Export

        public JSON Export()
        {
            JSON root = JSON.CreateObject();

            JSON components = JSON.CreateArray();
            foreach (var componentInfo in m_Components.Values)
                components.Add(componentInfo.Export());

            JSON triggers = JSON.CreateArray();
            foreach (var triggerInfo in m_Triggers.Values)
                triggers.Add(triggerInfo.Export());

            JSON queries = JSON.CreateArray();
            foreach (var queryInfo in m_Queries.Values)
                queries.Add(queryInfo.Export());

            JSON actions = JSON.CreateArray();
            foreach (var actioninfo in m_Actions.Values)
                actions.Add(actioninfo.Export());

            JSON groups = JSON.CreateArray();
            foreach (var groupInfo in m_Groups.Values)
                groups.Add(groupInfo.Export());

            root["components"] = components;
            root["triggers"] = triggers;
            root["queries"] = queries;
            root["actions"] = actions;
            root["groups"] = groups;

            return root;
        }

        #endregion // Export

        #region Assemblies

        static private IEnumerable<Assembly> GetAssembliesToScan()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                AssemblyName name = assembly.GetName();
                bool bAllow = true;
                foreach (var entry in ASSEMBLY_NAME_START_BLACKLIST)
                {
                    if (name.Name.StartsWith(entry))
                    {
                        bAllow = false;
                        break;
                    }
                }

                if (bAllow)
                {
                    yield return assembly;
                }
            }
        }

        static private readonly string[] ASSEMBLY_NAME_START_BLACKLIST = new string[]
        {
            "mscorlib",
            "System",
            "Mono.",
            "nunit.framework",
            "Unity"
        };

        #endregion // Assemblies
    }
}