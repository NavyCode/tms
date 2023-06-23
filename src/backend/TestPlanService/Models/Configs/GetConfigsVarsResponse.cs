using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class GetConfigsVarsResponse
    {
        public List<ConfigVarItem> Vars { get; set; } = new List<ConfigVarItem>();
    }
     
    public class ConfigVarItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comment { get; set; }

        public List<ConfigVarValueItem> Values { get; set; } = new List<ConfigVarValueItem>();
    }

    public class ConfigVarValueItem
    {
        public int Id { get; set; } 
        public string Value { get; set; }
    }

}
