using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class SetTestSuiteConfigRequest
    { 
        public int[] TestConfigIds { get; set; }
    }
}
