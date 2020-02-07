using System.Collections.Generic;
using BeauUtil;

namespace RuleScript.Data
{
    public interface IRSEntityLinkSet
    {
        int Count { get; }

        IEnumerable<KeyValuePair<RSEntityId, string>> AllLinks();

        RSEntityId EntityWithLink(StringSlice inLinkId);
        IEnumerable<RSEntityId> EntitiesWithLink(StringSlice inLinkId);

        bool AddLink(RSEntityId inEntity, string inLinkId);
        bool RemoveLink(RSEntityId inEntity, string inLinkId);

        bool RemoveAllLinks(RSEntityId inEntity);
        bool RemoveAllLinks(string inLinkId);

        bool Clear();
    }
}