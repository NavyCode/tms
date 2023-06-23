using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{
    public class AddOrUpdateProjectRequest
    {
        public string Name { get; set; }
        public string Description { get;  set; }
    }
}
