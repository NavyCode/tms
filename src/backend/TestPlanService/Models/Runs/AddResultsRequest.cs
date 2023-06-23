using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{  
    public class TestResultRequest
    { 
        public string AutomatedTestId { get; set; }

        public string AutomatedTestName { get; set; }

        public string AutomatedTestStorage { get; set; }

        public string ComputerName { get; set; }

        public DateTime StartedDate { get; set; }

        public string ErrorMessage { get; set; }

        public Outcome OutCome { get; set; }

        public string stackTrace { get; set; }

        public DateTime CompletedDate { get; set; }
        public string Description { get;  set; }
    }
}
