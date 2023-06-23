using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class TestConfigVariable
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public virtual ICollection<TestConfigVariableParam> Params { get; set; } = new List<TestConfigVariableParam>();

        public virtual Project Project { get; set; }
    }
}