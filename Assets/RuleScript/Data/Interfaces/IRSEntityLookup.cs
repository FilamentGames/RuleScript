using System;
using System.Collections.Generic;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    public interface IRSEntityLookup<out T> where T : class, IRSEntity
    {
        IEnumerable<T> AllEntities();

        T EntityWithId(RSEntityId inId);

        T EntityWithName(string inName);
        IEnumerable<T> EntitiesWithName(string inName);

        T EntityWithGroup(RSGroupId inGroup);
        IEnumerable<T> EntitiesWithGroup(RSGroupId inGroup);

        T EntityWithPrefab(string inPrefab);
        IEnumerable<T> EntitiesWithPrefab(string inPrefab);
    }

    static public class IRSEntityLookupExt
    {
        static public int CountEntitiesWithName<T>(this IRSEntityLookup<T> inEntityLookup, string inName) where T : class, IRSEntity
        {
            int count = 0;
            foreach(var entity in inEntityLookup.EntitiesWithName(inName))
            {
                ++count;
            }
            return count;
        }

        static public int CountEntitiesWithGroup<T>(this IRSEntityLookup<T> inEntityLookup, RSGroupId inGroup) where T : class, IRSEntity
        {
            int count = 0;
            foreach(var entity in inEntityLookup.EntitiesWithGroup(inGroup))
            {
                ++count;
            }
            return count;
        }

        static public int CountEntitiesWithPrefab<T>(this IRSEntityLookup<T> inEntityLookup, string inPrefab) where T : class, IRSEntity
        {
            int count = 0;
            foreach(var entity in inEntityLookup.EntitiesWithPrefab(inPrefab))
            {
                ++count;
            }
            return count;
        }
    }
}