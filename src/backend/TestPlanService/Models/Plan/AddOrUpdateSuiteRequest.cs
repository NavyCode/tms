using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class AddOrUpdateSuiteRequest
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
    }
}
