using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{

    public class TestResult
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual TestRun TestRun { get; set; }

        public string AutomatedTestId { get; set; }

        public string AutomatedTestName { get; set; }

        public string AutomatedTestStorage { get; set; }

        public string Description { get; set; }

        public string ComputerName { get; set; }

        public DateTime StartedDate { get; set; }


        public string ErrorMessage { get; set; }

        public Outcome OutCome { get; set; }

        public virtual User Owner { get; set; }

        public string stackTrace { get; set; }

        public DateTime CompletedDate { get; set; }

        public virtual ICollection<File> Files { get; set; } = new List<File>();
    }
}