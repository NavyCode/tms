using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetConfigsForAssignResponse
    {
        public List<ConfigsForAssign> Configs { get; set; } = new List<ConfigsForAssign>();
    }
     
    public class ConfigsForAssign
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Values { get; set; }
    }
}
