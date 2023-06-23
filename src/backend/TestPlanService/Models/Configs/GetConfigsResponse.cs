using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetConfigsResponse
    {
        public List<ConfigItem> Configs { get; set; } = new List<ConfigItem>();
    }
     
    public class ConfigItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool IsDefault { get; set; }

        public List<ConfigVariableItem> Variables { get; set; } = new List<ConfigVariableItem>();
} 
    public class ConfigVariableItem
    {
        public int Id { get; set; }
        public int VariableId { get; set; }
        public int ValueId { get; set; }
    } 
}
