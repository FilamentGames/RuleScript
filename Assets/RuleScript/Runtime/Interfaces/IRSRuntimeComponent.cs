using RuleScript.Data;

namespace RuleScript.Runtime
{
    public interface IRSRuntimeComponent : IRSComponent
    {
        new IRSRuntimeEntity Entity { get; }
    }
}