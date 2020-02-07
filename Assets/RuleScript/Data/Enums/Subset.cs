namespace RuleScript.Data
{
    /// <summary>
    /// Defines what subset of elements must pass
    /// </summary>
    public enum Subset : byte
    {
        // All elements must pass
        All = 0,

        // Any element must pass
        Any = 1,

        // No elements must pass
        None = 2
    }
}