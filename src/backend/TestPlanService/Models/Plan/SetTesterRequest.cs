using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class SetTesterRequest 
    {   
        public int[] TestPointIds { get; set; }
        public int TesterId { get; set; }
    }
}
