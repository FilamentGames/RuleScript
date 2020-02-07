using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Runtime
{
    public interface IScriptContext
    {
        IRSDebugLogger Logger { get; }
        IRSEntityMgr Entities { get; }
        IRSRuleTableMgr Tables { get; }
        RSLibrary Library { get; }

        void Trigger(IRSRuntimeEntity inEntity, RSTriggerId inTriggerId, object inArgument = null, bool inbForce = false);
        void Broadcast(RSTriggerId inTriggerId, object inArgument = null, bool inbForce = false);

        void LoadRegister(RegisterIndex inRegister, object inValue);
        object PeekRegister(RegisterIndex inRegister);
    }
}