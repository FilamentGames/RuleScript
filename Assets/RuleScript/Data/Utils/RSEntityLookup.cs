using System;
using System.Collections.Generic;
using BeauData;
using BeauRoutine;
using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    public sealed class RSEntityLookup<T> : IRSEntityLookup<T> where T : class, IRSEntity
    {
        private Dictionary<RSEntityId, T> m_IdMap = new Dictionary<RSEntityId, T>();
        private Dictionary<RSGroupId, HashSet<T>> m_GroupMap = new Dictionary<RSGroupId, HashSet<T>>();
        private Dictionary<string, HashSet<T>> m_NameMap = new Dictionary<string, HashSet<T>>();
        private Dictionary<string, HashSet<T>> m_PrefabMap = new Dictionary<string, HashSet<T>>();

        public IEnumerable<T> AllEntities()
        {
            foreach (var entity in m_IdMap.Values)
                yield return entity;
        }

        #region Lookup by Id

        public T EntityWithId(RSEntityId inId)
        {
            T entity;
            m_IdMap.TryGetValue(inId, out entity);
            return entity;
        }

        #endregion // Lookup by Id

        #region Group

        public T EntityWithGroup(RSGroupId inGroup)
        {
            HashSet<T> groupSet;
            if (m_GroupMap.TryGetValue(inGroup, out groupSet))
            {
                var enumerator = groupSet.GetEnumerator();
                if (enumerator.MoveNext())
                    return enumerator.Current;
            }

            return null;
        }

        public IEnumerable<T> EntitiesWithGroup(RSGroupId inGroup)
        {
            HashSet<T> groupMap;
            if (m_GroupMap.TryGetValue(inGroup, out groupMap))
            {
                foreach (var entity in groupMap)
                    yield return entity;
            }
        }

        #endregion // Group

        #region Name

        public T EntityWithName(string inName)
        {
            foreach (var kv in m_NameMap)
            {
                if (ScriptUtils.StringMatch(kv.Key, inName))
                {
                    var enumerator = kv.Value.GetEnumerator();
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }

            return null;
        }

        public IEnumerable<T> EntitiesWithName(string inName)
        {
            foreach (var kv in m_NameMap)
            {
                if (ScriptUtils.StringMatch(kv.Key, inName))
                {
                    foreach (var entity in kv.Value)
                        yield return entity;
                }
            }
        }

        #endregion // Name

        #region Prefab

        public T EntityWithPrefab(string inPrefab)
        {
            foreach (var kv in m_PrefabMap)
            {
                if (ScriptUtils.StringMatch(kv.Key, inPrefab))
                {
                    var enumerator = kv.Value.GetEnumerator();
                    if (enumerator.MoveNext())
                        return enumerator.Current;
                }
            }

            return null;
        }

        public IEnumerable<T> EntitiesWithPrefab(string inPrefab)
        {
            foreach (var kv in m_PrefabMap)
            {
                if (ScriptUtils.StringMatch(kv.Key, inPrefab))
                {
                    foreach (var entity in kv.Value)
                        yield return entity;
                }
            }
        }

        #endregion // Prefab

        #region Registration

        public bool Add(IEnumerable<T> inEntities)
        {
            bool bAdded = false;
            foreach (var entity in inEntities)
                bAdded |= Add(entity);
            return bAdded;
        }

        public bool Add(T inEntity)
        {
            if (m_IdMap.ContainsKey(inEntity.Id))
                return false;

            m_IdMap.Add(inEntity.Id, inEntity);
            AddToGroup(inEntity, inEntity.Group);
            AddToName(inEntity, inEntity.Name);
            AddToPrefab(inEntity, inEntity.Prefab);
            return true;
        }

        public bool Remove(T inEntity)
        {
            if (!m_IdMap.ContainsKey(inEntity.Id))
                return false;

            RemoveFromPrefab(inEntity, inEntity.Prefab);
            RemoveFromName(inEntity, inEntity.Name);
            RemoveFromGroup(inEntity, inEntity.Group);
            m_IdMap.Remove(inEntity.Id);
            return true;
        }

        public bool AddToGroup(T inEntity, RSGroupId inGroup)
        {
            HashSet<T> groupSet;
            if (!m_GroupMap.TryGetValue(inGroup, out groupSet))
            {
                groupSet = new HashSet<T>();
                m_GroupMap.Add(inGroup, groupSet);
            }
            return groupSet.Add(inEntity);
        }

        public bool RemoveFromGroup(T inEntity, RSGroupId inGroup)
        {
            HashSet<T> groupSet;
            if (m_GroupMap.TryGetValue(inGroup, out groupSet))
            {
                return groupSet.Remove(inEntity);
            }
            return false;
        }

        public bool AddToName(T inEntity, string inName)
        {
            HashSet<T> groupSet;
            if (!m_NameMap.TryGetValue(inName, out groupSet))
            {
                groupSet = new HashSet<T>();
                m_NameMap.Add(inName, groupSet);
            }
            return groupSet.Add(inEntity);
        }

        public bool RemoveFromName(T inEntity, string inName)
        {
            HashSet<T> groupSet;
            if (m_NameMap.TryGetValue(inName, out groupSet))
            {
                return groupSet.Remove(inEntity);
            }
            return false;
        }

        public bool AddToPrefab(T inEntity, string inPrefab)
        {
            HashSet<T> groupSet;
            if (!m_PrefabMap.TryGetValue(inPrefab, out groupSet))
            {
                groupSet = new HashSet<T>();
                m_PrefabMap.Add(inPrefab, groupSet);
            }
            return groupSet.Add(inEntity);
        }

        public bool RemoveFromPrefab(T inEntity, string inPrefab)
        {
            HashSet<T> groupSet;
            if (m_PrefabMap.TryGetValue(inPrefab, out groupSet))
            {
                return groupSet.Remove(inEntity);
            }
            return false;
        }

        public void Clear()
        {
            m_IdMap.Clear();
            m_NameMap.Clear();
            m_PrefabMap.Clear();
            m_GroupMap.Clear();
        }

        #endregion // Registration
    }
}