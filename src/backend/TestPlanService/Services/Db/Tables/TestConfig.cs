using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class TestConfig : TableBase
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public virtual ICollection<TestConfigParam> Params { get; set; } = new List<TestConfigParam>();

        public virtual Project Project { get; set; }

        public bool IsDefault { get; internal set; }
    }
}