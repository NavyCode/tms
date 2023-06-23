using System;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.TestCase
{ 
    public class SearchTestCaseItem
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public int Priority { get; private set; }

        internal static SearchTestCaseItem FromDb(WorkItem t)
        {
            return new SearchTestCaseItem()
            {
                Id = t.Id,
                Name = t.Revision.Title,
                Priority = t.Revision.Priority
            };
        }
    }
}
