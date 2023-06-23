using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class AddTestCaseToSuiteRequest
    {
        public int[] TestCasesIds { get; set; }
    }
}
