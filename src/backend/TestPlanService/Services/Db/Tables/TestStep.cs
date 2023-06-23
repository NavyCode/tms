using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace TestPlanService
{
    public class TestStep
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Order { get; set; }

        public string Action { get; set; }

        public string Result { get; set; }
    }
}