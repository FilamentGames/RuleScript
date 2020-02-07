using System;
using RuleScript.Data;
using RuleScript.Metadata;

namespace RuleScript.Validation
{
    static public class RSValidator
    {
        /// <summary>
        /// Attempts to validate the given table.
        /// </summary>
        static public RSValidationState Validate(RSRuleTableData inRuleTable, RSValidationContext inContext)
        {
            RSValidationState state = new RSValidationState(inRuleTable?.Name ?? "Rule Table");
            ValidationLogic.ValidateTable(inRuleTable, state, inContext);
            state.Finish();
            return state;
        }
    }
}