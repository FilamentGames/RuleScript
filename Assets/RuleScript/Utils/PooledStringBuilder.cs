using System;
using System.Collections.Generic;
using System.Text;

namespace RuleScript
{
    internal sealed class PooledStringBuilder : IDisposable
    {
        public readonly StringBuilder Builder;

        private PooledStringBuilder()
        {
            Builder = new StringBuilder(1024);
        }

        private void OnAlloc()
        {
            Builder.Length = 0;
        }

        void IDisposable.Dispose()
        {
            Builder.Length = 0;
            s_Pool.Push(this);
        }

        public override string ToString()
        {
            return Builder.ToString();
        }

        #region Pool

        static private Stack<PooledStringBuilder> s_Pool = new Stack<PooledStringBuilder>(64);

        static public PooledStringBuilder Alloc()
        {
            PooledStringBuilder builder = null;
            if (s_Pool.Count > 0)
                builder = s_Pool.Pop();
            else
                builder = new PooledStringBuilder();
            builder.OnAlloc();

            return builder;
        }

        static public PooledStringBuilder Alloc(string inCopy)
        {
            PooledStringBuilder builder = Alloc();
            builder.Builder.Append(inCopy);
            return builder;
        }

        #endregion // Pool
    }
}