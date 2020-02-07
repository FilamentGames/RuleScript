using RuleScript.Metadata;

namespace RuleScript.Data
{
    public interface IRSPreviewable
    {
        string GetPreviewString(RSTriggerInfo inTriggerContext, RSLibrary inLibrary);
    }
}