using System.Collections.Generic;
using BeauUtil;
using RuleScript.Data;

namespace RuleScript.Runtime
{
    public interface IRSRuntimeEntityLinkSet<out T> : IRSEntityLinkSet where T : class, IRSRuntimeEntity
    {
        new T EntityWithLink(StringSlice inLinkId);
        new IEnumerable<T> EntitiesWithLink(StringSlice inLinkId);
    }
}