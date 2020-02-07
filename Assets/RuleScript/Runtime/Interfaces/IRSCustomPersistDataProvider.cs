using BeauData;
using RuleScript.Data;

namespace RuleScript.Runtime
{
    public interface IRSCustomPersistDataProvider
    {
        void GetCustomPersistData(ref IRSCustomPersistData ioObject, int inFlags);
        void RestoreCustomPersistData(IRSCustomPersistData inObject, int inFlags, IScriptContext inContext);
    }
}