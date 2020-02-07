using System;

namespace RuleScript.Data
{
    /// <summary>
    /// Additional flags determining how a rule functions.
    /// </summary>
    [Flags]
    public enum RuleFlags
    {
        // This rule uses registers for storage/retrieval
        UsesRegisters = 0x001
    }
}