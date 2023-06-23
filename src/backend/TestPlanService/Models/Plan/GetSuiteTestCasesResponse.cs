using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetSuiteTestCasesResponse
    {
        public List<SuiteTestCaseItem> Tests { get; set; } = new List<SuiteTestCaseItem>();
    }
     
    public class SuiteTestCaseItem
    {
        public int Id { get; set; }
        public int TestCaseId { get; set; } 
        public string Name { get; set; }
        public int Order { get; set; }
        public WiState State { get; set; } 
        public int Priority { get; set; }
    }
}
