using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Suites
{
    public class AddOrUpdateConfigsRequest
    {
        public string Name { get; set; }
        public string Comment { get; set; }
        public bool IsDefault { get; set; }

        public List<AddOrUpdateConfigVariable> Variables { get; set; } = new List<AddOrUpdateConfigVariable>();
} 
    public class AddOrUpdateConfigVariable
    {
        public int? Id { get; set; }
        public int VariableId { get; set; }
        public int ValueId { get; set; }
    } 
}
