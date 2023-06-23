using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TestPlanService.Services.Db.Tables
{
    public class TestConfigVariableParam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Value { get; set; }

        public virtual TestConfigVariable ConfigVar { get; set; }
    }
}