namespace RuleScript.Data
{
    /// <summary>
    /// Determines how a ResolvableValue should be resolved.
    /// </summary>
    public enum ResolvableValueMode : byte
    {
        // A fixed value
        Value = 0,

        // Result of an object query
        Query = 1,

        // Argument passed into the trigger
        Argument = 2,

        // Local variable
        Register = 3
    }
}