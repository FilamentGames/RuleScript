using System;
using System.Collections;
using System.Collections.Generic;

namespace RuleScript.Runtime
{
    internal struct MultiReturn<T> : IEnumerable<T>
    {
        public readonly T Single;
        public readonly IEnumerable<T> Set;

        public MultiReturn(T inSingle)
        {
            Single = inSingle;
            Set = null;
        }

        public MultiReturn(IEnumerable<T> inSet)
        {
            Single = default(T);
            Set = inSet;
        }

        public T ForceSingle()
        {
            if (Set == null)
                return Single;
            
            var enumerator = Set.GetEnumerator();
            Assert.True(enumerator != null, "Result set {0} has null enumerator", typeof(T).Name);
            
            T result;
            if (enumerator.MoveNext())
            {
                result = enumerator.Current;
            }
            else
            {
                result = default(T);
            }

            ((IDisposable) enumerator).Dispose();
            return result;
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            if (Set != null)
                return Set.GetEnumerator();

            if (Single != null)
                return YieldSingle(Single);

            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        static private IEnumerator<T> YieldSingle(T inResult)
        {
            yield return inResult;
        }

        #endregion // IEnumerable

        static public implicit operator MultiReturn<T>(T inSingle)
        {
            return new MultiReturn<T>(inSingle);
        }

        static public implicit operator T(MultiReturn<T> inValue)
        {
            return inValue.ForceSingle();
        }

        static public readonly MultiReturn<T> Default = new MultiReturn<T>();
    }
}