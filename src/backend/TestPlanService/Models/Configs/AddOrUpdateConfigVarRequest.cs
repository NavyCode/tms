using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{ 
    public class AddOrUpdateConfigVarRequest
    {
        public string Name { get; set; }
        public string Comment { get; set; }

        public List<AddOrUpdateConfigVarValue> Values { get; set; } = new List<AddOrUpdateConfigVarValue>();
    }

    public class AddOrUpdateConfigVarValue
    {
        public int? Id { get; set; }
        public string Value { get; set; }
    }

}
