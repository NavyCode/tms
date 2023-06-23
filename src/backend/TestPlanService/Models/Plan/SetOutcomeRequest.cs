using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class SetOutcomeRequest 
    {   
        public int[] TestPointIds { get; set; }
        public Outcome Outcome { get; set; }
    }


    public class SetDescriptionRequest
    {
        public int[] TestPointIds { get; set; }
        public string Description { get; set; }
    }
}
