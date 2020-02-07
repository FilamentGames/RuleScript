using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Validation
{
    public struct RSValidationContext
    {
        /// <summary>
        /// Library.
        /// </summary>
        public readonly RSLibrary Library;

        /// <summary>
        /// Entity manager.
        /// </summary>
        public readonly IRSEntityMgr Manager;

        /// <summary>
        /// Editing entity.
        /// </summary>
        public readonly IRSEntity Entity;

        /// <summary>
        /// Trigger for the current rule.
        /// </summary>
        public readonly RSTriggerInfo Trigger;

        /// <summary>
        /// Info for the current parameter.
        /// </summary>
        public readonly RSParameterInfo Parameter;

        internal RSValidationContext(RSLibrary inLibrary, IRSEntityMgr inManager, IRSEntity inEntity, RSTriggerInfo inTrigger, RSParameterInfo inParameterInfo)
        {
            Library = inLibrary;
            Manager = inManager;
            Entity = inEntity;
            Trigger = inTrigger;
            Parameter = inParameterInfo;
        }

        public RSValidationContext(RSLibrary inLibrary) : this(inLibrary, null, null, null, null) { }

        public RSValidationContext WithLibrary(RSLibrary inLibrary)
        {
            return new RSValidationContext(inLibrary, Manager, Entity, Trigger, Parameter);
        }

        public RSValidationContext WithManager(IRSEntityMgr inManager)
        {
            return new RSValidationContext(Library, inManager, Entity, Trigger, Parameter);
        }

        public RSValidationContext WithEntity(IRSEntity inEntity)
        {
            return new RSValidationContext(Library, Manager, inEntity, Trigger, Parameter);
        }

        public RSValidationContext WithTrigger(RSTriggerInfo inTrigger)
        {
            return new RSValidationContext(Library, Manager, Entity, inTrigger, Parameter);
        }

        public RSValidationContext WithParameter(RSParameterInfo inParameter)
        {
            return new RSValidationContext(Library, Manager, Entity, Trigger, inParameter);
        }
    }
}