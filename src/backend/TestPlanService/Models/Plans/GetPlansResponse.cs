using System;
using System.Collections.Generic;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Plans
{
    public class GetPlansResponse
    {
        public List<TestPlanItem> Plans { get; set; } = new List<TestPlanItem>();
    }

    public class TestPlanItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
        public PlanState State { get; set; }
        public UserIdentity AssignedTo { get; set; }

        internal static TestPlanItem FromDb(TestPlan item)
        {
            return new TestPlanItem()
            {
                Id = item.Id,
                AssignedTo = new UserIdentity(item.AssignedTo ?? item.CreatedBy),
                Comment = item.Description,
                Name = item.Title,
                State = item.State
            };
        }
    }
}
