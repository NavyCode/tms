using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Plans
{
    public class AddOrUpdatePlanRequest
    {
        public string Name { get; set; }
        public string Description { get;  set; }
        public PlanState State { get; set; } 
        public int AssignedTo { get; set; }
    }
}
