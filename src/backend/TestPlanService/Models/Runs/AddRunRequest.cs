using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{
    public class AddRunRequest
    {
        public bool IsAutomated { get; set; }

        public string Name { get; set; }
    }
}
