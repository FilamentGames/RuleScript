using System.Collections.Generic;
using RuleScript.Data;

namespace RuleScript.Runtime
{
    /// <summary>
    /// Map of entities listening for specific triggers.
    /// </summary>
    public sealed class RSTriggerListenerMap<T> where T : IRSEntity
    {
        private readonly Dictionary<int, HashSet<T>> m_Map;

        public RSTriggerListenerMap()
        {
            m_Map = new Dictionary<int, HashSet<T>>();
        }

        /// <summary>
        /// Registers the given entity as a listener on the given trigger id.
        /// </summary>
        public void Register(T inEntity, RSTriggerId inTrigger)
        {
            GetHashSet(inTrigger).Add(inEntity);
        }

        /// <summary>
        /// Deregisters the given entity as a listener on the given trigger id.
        /// </summary>
        public void Deregister(T inEntity, RSTriggerId inTrigger)
        {
            GetHashSet(inTrigger, false)?.Remove(inEntity);
        }

        /// <summary>
        /// Outputs all entities listening for the given trigger to a collection.
        /// </summary>
        public int GetListeners(RSTriggerId inTrigger, ICollection<T> outCollection)
        {
            HashSet<T> set = GetHashSet(inTrigger, false);
            if (set == null)
                return 0;

            foreach (var entity in set)
                outCollection.Add(entity);

            return set.Count;
        }

        private HashSet<T> GetHashSet(RSTriggerId inTriggerId, bool inbCreate = true)
        {
            HashSet<T> set;
            if (!m_Map.TryGetValue((int) inTriggerId, out set) && inbCreate)
            {
                set = new HashSet<T>(EntityEqualityComparer<T>.Instance);
                m_Map.Add((int) inTriggerId, set);
            }
            return set;
        }
    }
}