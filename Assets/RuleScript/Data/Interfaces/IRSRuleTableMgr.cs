using System;
using System.Collections.Generic;
using RuleScript.Metadata;

namespace RuleScript.Data
{
    public interface IRSRuleTableMgr
    {
        RSRuleTableData TableWithName(string inName);
        IEnumerable<RSRuleTableData> TablesWithName(string inName);

        IEnumerable<RSRuleTableData> AllTables();

        IEnumerable<IRSRuleTableSource> AllTableSources();
        bool RegisterTableSource(IRSRuleTableSource inTable);
        bool DeregisterTableSource(IRSRuleTableSource inTable);
    }
}