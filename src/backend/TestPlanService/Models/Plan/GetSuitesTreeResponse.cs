using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetSuitesTreeResponse
    {
        public List<SuitesTreeItem> Suites { get; set; } = new List<SuitesTreeItem>();
    }
     
    public class SuitesTreeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentId { get; set; }
    }

}
