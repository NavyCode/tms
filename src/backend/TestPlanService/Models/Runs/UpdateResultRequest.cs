using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{
    public class UpdateResultRequest
    {
        public int TestCaseResultId { get; set; }

        public string Comment { get; set; }
    }
}
