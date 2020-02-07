using System;

namespace RuleScript.Metadata
{
    [Flags]
    public enum TypeFlags
    {
        // enum type
        IsEnum = 0x001,

        // enum type is a "flags" enum
        IsFlags = 0x002,

        // entity type
        IsEntity = 0x004,

        // component type
        IsComponent = 0x008,

        // any type
        SkipConversionCheck = 0x010
    }
}