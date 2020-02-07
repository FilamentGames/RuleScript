using System.Collections.Generic;
using RuleScript.Data;

namespace RuleScript.Runtime
{
    public interface IRSRuntimeEntityMgr : IRSEntityMgr
    {
        new IRSEntityLookup<IRSRuntimeEntity> Lookup { get; }

        IEnumerable<IRSRuntimeEntity> EntitiesForTrigger(RSTriggerId inTrigger);

        IScriptContext Context { get; set; }

        void Initialize();
        bool IsReady();
        void Destroy();

        void RegisterTriggers(IRSRuntimeEntity inEntity, IEnumerable<RSTriggerId> inTriggers);
        void DeregisterTriggers(IRSRuntimeEntity inEntity, IEnumerable<RSTriggerId> inTriggers);
    }
}