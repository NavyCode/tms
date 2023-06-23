using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class SetTestCaseConfigRequest
    { 
        public int[] SuiteTestCaseIds { get; set; }
        public int[] TestConfigIds { get; set; }
    }
}
