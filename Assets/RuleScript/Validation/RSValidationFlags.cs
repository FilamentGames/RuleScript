using System;
using RuleScript.Metadata;

namespace RuleScript.Validation
{
    [Flags]
    public enum RSValidationFlags
    {
        None = 0,

        AllowNullEntity = 1 << 0,
        AllowGlobalEntity = 1 << 2,
        RequireSingleEntity = 1 << 3,
        DisallowDirectValue = 1 << 4,
        DisallowParameters = 1 << 5,
        DisallowRegisters = 1 << 6,

        FilterSelection = 1 << 31
    }

    static public class RSValidationFlagExtensions
    {
        static public RSValidationFlags Isolate(this RSValidationFlags inFlags, RSValidationFlags inIsolate)
        {
            return inFlags & inIsolate;
        }

        static public RSValidationFlags With(this RSValidationFlags inFlags, RSValidationFlags inWith)
        {
            return inFlags | inWith;
        }

        static public RSValidationFlags Without(this RSValidationFlags inFlags, RSValidationFlags inWithout)
        {
            return inFlags & ~inWithout;
        }

        static public bool Has(this RSValidationFlags inFlags, RSValidationFlags inCheck)
        {
            return (inFlags & inCheck) == inCheck;
        }

        static public RSValidationFlags ForConditionQuery(this RSValidationFlags inFlags)
        {
            inFlags &= RSValidationFlags.FilterSelection;
            inFlags |= RSValidationFlags.AllowGlobalEntity;
            inFlags |= RSValidationFlags.DisallowDirectValue;
            inFlags |= RSValidationFlags.DisallowRegisters;
            return inFlags;
        }

        static public RSValidationFlags ForConditionTarget(this RSValidationFlags inFlags)
        {
            inFlags &= RSValidationFlags.FilterSelection;
            inFlags |= RSValidationFlags.AllowGlobalEntity;
            inFlags |= RSValidationFlags.AllowNullEntity;
            inFlags |= RSValidationFlags.RequireSingleEntity;
            inFlags |= RSValidationFlags.DisallowRegisters;
            return inFlags;
        }

        static public RSValidationFlags ForParameter(this RSValidationFlags inFlags, RSParameterInfo inParameterInfo)
        {
            inFlags &= RSValidationFlags.FilterSelection;
            inFlags |= RSValidationFlags.RequireSingleEntity;
            if (!inParameterInfo.NotNull)
                inFlags |= RSValidationFlags.AllowNullEntity;
            return inFlags;
        }

        static public RSValidationFlags ForMethodScope(this RSValidationFlags inFlags)
        {
            inFlags |= RSValidationFlags.AllowGlobalEntity;
            inFlags &= ~RSValidationFlags.AllowNullEntity;
            return inFlags;
        }

        static public RSValidationFlags ForEntityValue(this RSValidationFlags inFlags)
        {
            inFlags |= RSValidationFlags.RequireSingleEntity;
            inFlags &= ~RSValidationFlags.AllowGlobalEntity;
            return inFlags;
        }

        static public RSValidationFlags ForMethod(this RSValidationFlags inFlags, bool inbAllowParameters)
        {
            inFlags &= ~RSValidationFlags.AllowNullEntity;
            inFlags |= RSValidationFlags.AllowGlobalEntity;
            if (inbAllowParameters)
                inFlags &= ~RSValidationFlags.DisallowParameters;
            else
                inFlags |= RSValidationFlags.DisallowParameters;
            return inFlags;
        }
    }
}