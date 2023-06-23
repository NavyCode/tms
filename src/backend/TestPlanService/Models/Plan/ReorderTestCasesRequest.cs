using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class ReorderTestCasesRequest
    {  
        public Dictionary<int, int> SuiteTestCaseOrderDictionary { get; set; }
    }
}
