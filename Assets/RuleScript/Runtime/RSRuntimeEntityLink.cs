using System;

namespace RuleScript.Runtime
{
    [Serializable]
    public abstract class RSRuntimeEntityLink<T> where T : class, IRSRuntimeEntity
    {
        public T Entity;
        public string Name;

        public RSRuntimeEntityLink(T inEntity, string inName)
        {
            Entity = inEntity;
            Name = inName;
        }
    }
}