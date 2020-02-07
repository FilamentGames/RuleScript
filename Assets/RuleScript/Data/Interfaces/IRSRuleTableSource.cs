using System;
using System.Collections.Generic;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    public interface IRSRuleTableSource
    {
        RSRuleTableData TableData { get; }
    }
}