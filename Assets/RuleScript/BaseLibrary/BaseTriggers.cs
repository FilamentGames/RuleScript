using RuleScript.Data;
using RuleScript.Runtime;
using UnityEngine;

namespace RuleScript.Library
{
    static public class BaseTriggers
    {
        #region Entity

        [RSTrigger("entity:active-changed", Name = "Entity/Active Changed",
            Description = "Triggers when the active state of the entity changes",
            OwnerType = typeof(IRSRuntimeEntity),
            ParameterName = "IsActive", ParameterType = typeof(bool))]
        static public readonly RSTriggerId Entity_ActiveChanged;

        [RSTrigger("entity:activated", Name = "Entity/Activated",
            Description = "Triggers when the entity is activated",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Activated;

        [RSTrigger("entity:deactivated", Name = "Entity/Deactivated",
            Description = "Triggers when the entity is deactivated",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Deactivated;

        [RSTrigger("entity:initialized", Name = "Entity/Initialized",
            Description = "Triggers when the entity is initialized",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Initialized;

        [RSTrigger("entity:destroyed", Name = "Entity/Destroyed",
            Description = "Triggers when the entity is destroyed",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Destroyed;

        #endregion // Entity

        #region Custom

        [RSTrigger("custom:custom-0", Name = "Custom/Custom 0",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom0;

        [RSTrigger("custom:custom-1", Name = "Custom/Custom 1",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom1;

        [RSTrigger("custom:custom-2", Name = "Custom/Custom 2",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom2;

        [RSTrigger("custom:custom-3", Name = "Custom/Custom 3",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom3;

        [RSTrigger("custom:custom-4", Name = "Custom/Custom 4",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom4;

        [RSTrigger("custom:custom-5", Name = "Custom/Custom 5",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom5;

        [RSTrigger("custom:custom-6", Name = "Custom/Custom 6",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom6;

        [RSTrigger("custom:custom-7", Name = "Custom/Custom 7",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom7;

        [RSTrigger("custom:custom-8", Name = "Custom/Custom 8",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom8;

        [RSTrigger("custom:custom-9", Name = "Custom/Custom 9",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom9;

        [RSTrigger("custom:custom-10", Name = "Custom/Custom 10",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom10;

        [RSTrigger("custom:custom-11", Name = "Custom/Custom 11",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom11;

        [RSTrigger("custom:custom-12", Name = "Custom/Custom 12",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom12;

        [RSTrigger("custom:custom-13", Name = "Custom/Custom 13",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom13;

        [RSTrigger("custom:custom-14", Name = "Custom/Custom 14",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom14;

        [RSTrigger("custom:custom-15", Name = "Custom/Custom 15",
            Description = "Custom trigger. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity))]
        static public readonly RSTriggerId Entity_Custom15;

        [RSTrigger("custom:custom-string", Name = "Custom/Custom String",
            Description = "Custom trigger with a string parameter. Must be triggered manually in RuleScript",
            OwnerType = typeof(IRSRuntimeEntity),
            ParameterType = typeof(string), ParameterName = "Custom String")]
        static public readonly RSTriggerId Entity_CustomString;

        #endregion // Custom
    }
}