using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class DeleteTestCaseFromSuiteRequest
    {
        public int[] SuiteTestCasesIds { get; set; }
    }
}
