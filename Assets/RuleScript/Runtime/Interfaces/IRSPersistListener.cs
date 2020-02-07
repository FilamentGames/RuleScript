namespace RuleScript.Runtime
{
    public interface IRSPersistListener
    {
        void OnPrePersist();
        void OnPostPersist();

        void OnPreRestore(IScriptContext inContext);
        void OnPostRestore(IScriptContext inContext);
    }
}