using System;

namespace RuleScript.Metadata
{
    internal enum InvocationTarget
    {
        // Use null
        Null = 0,

        // Use the entity
        Entity,

        // Use the component
        Component,

        // Use the scripting utility
        Context,
    }

    static internal class InvocationTargetExtensions
    {
        static public bool MustBeNotNull(this InvocationTarget inTarget)
        {
            switch(inTarget)
            {
                case InvocationTarget.Null:
                    return false;

                default:
                    return true;
            }
        }
    }
}