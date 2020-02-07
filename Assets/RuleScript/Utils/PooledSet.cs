using System;
using System.Collections.Generic;
using System.Text;

namespace RuleScript
{
    internal sealed class PooledSet<T> : HashSet<T>, IDisposable
    {
        private void OnAlloc()
        {
            Clear();
        }

        void IDisposable.Dispose()
        {
            Clear();
            s_Pool.Push(this);
        }

        #region Pool

        static private Stack<PooledSet<T>> s_Pool = new Stack<PooledSet<T>>(64);

        static public PooledSet<T> Alloc()
        {
            PooledSet<T> set = null;
            if (s_Pool.Count > 0)
                set = s_Pool.Pop();
            else
                set = new PooledSet<T>();
            set.OnAlloc();

            return set;
        }

        static public PooledSet<T> Alloc(IEnumerable<T> inSource)
        {
            PooledSet<T> set = Alloc();
            foreach(var element in inSource)
                set.Add(element);
            return set;
        }

        #endregion // Pool
    }
}