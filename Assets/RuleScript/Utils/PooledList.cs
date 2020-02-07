using System;
using System.Collections.Generic;
using System.Text;

namespace RuleScript
{
    internal sealed class PooledList<T> : List<T>, IDisposable
    {
        private void OnAlloc()
        {
            Clear();
        }

        public void Dispose()
        {
            Clear();
            s_Pool.Push(this);
        }

        #region Pool

        static private Stack<PooledList<T>> s_Pool = new Stack<PooledList<T>>(64);

        static public PooledList<T> Alloc()
        {
            PooledList<T> list = null;
            if (s_Pool.Count > 0)
                list = s_Pool.Pop();
            else
                list = new PooledList<T>();
            list.OnAlloc();

            return list;
        }

        static public PooledList<T> Alloc(IEnumerable<T> inSource)
        {
            PooledList<T> list = Alloc();
            foreach(var element in inSource)
                list.Add(element);
            return list;
        }

        #endregion // Pool
    }
}