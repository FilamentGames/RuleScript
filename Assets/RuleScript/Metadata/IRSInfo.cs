using BeauData;

namespace RuleScript.Metadata
{
    /// <summary>
    /// Metadata for a rule scripting element.
    /// </summary>
    public interface IRSInfo
    {
        string Id { get; }
        int IdHash { get; }

        string Name { get; }
        string Description { get; }
        string Icon { get; }
        string Tooltip { get; }

        JSON Export();
    }
}