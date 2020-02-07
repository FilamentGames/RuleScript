using System;
using System.Collections.Generic;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    public interface IRSEntity
    {
        RSEntityId Id { get; set; }
        string Prefab { get; }
        string Name { get; set; }
        RSGroupId Group { get; set; }

        IRSEntityMgr Manager { get; set; }

        IRSEntityLinkSet Links { get; }

        IEnumerable<Type> GetRSComponentTypes(RSLibrary inLibrary);
    }
}