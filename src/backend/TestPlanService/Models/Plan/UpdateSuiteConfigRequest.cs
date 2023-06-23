using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class UpdateSuiteConfigRequest
    {
        public int SuiteId { get; set; }
        public int[] TestConfigIds { get; set; }
    }
}
