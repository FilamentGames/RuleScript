using System.Collections.Generic;
using RuleScript.Data;

namespace RuleScript
{
    /// <summary>
    /// Equality comparer for IRSEntity
    /// </summary>
    internal sealed class EntityEqualityComparer<T> : IEqualityComparer<T> where T : IRSEntity
    {
        static public readonly EntityEqualityComparer<T> Instance = new EntityEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            if (x == null)
                return y == null;

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;

            return obj.Id.GetHashCode();
        }
    }
}