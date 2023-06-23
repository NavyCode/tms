using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TestPlanService.Services.Db.Tables
{
    public class TestConfigParam : TableBase
    { 
        public virtual TestConfigVariable Variable { get; set; }

        public virtual TestConfigVariableParam VariableParam { get; set; }

        public virtual TestConfig Config { get; set; }
    }
}