using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetSuiteTestPointsResponse
    {
        public List<SuiteTestPointItem> Points { get; set; } = new List<SuiteTestPointItem>();
    }
     
    public class SuiteTestPointItem
    {
        public int Id { get; set; }
        public int TestSuiteId { get; set; }
        public int TestConfigId { get; set; }
        public int TestCaseId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int Priority { get; set; }
        public Outcome Outcome { get; set; }
        public string Configuration { get; set; }
        public string Tester { get; set; }
        public string Description { get; set; }
    }
}
