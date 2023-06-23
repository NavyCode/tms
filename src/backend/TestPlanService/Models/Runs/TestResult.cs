using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TestPlanService.Models.Authorization;

namespace TestPlanService.Services.Db.Tables
{

    public class TestRunPoint
    {
        public int Id { get; set; }

        public string AutomatedTestId { get; set; }

        public string AutomatedTestName { get; set; }

        public string AutomatedTestStorage { get; set; }

        public string Description { get; set; } 

        public string ComputerName { get; set; }

        public DateTime StartedDate { get; set; } 

        public string ErrorMessage { get; set; }

        public Outcome OutCome { get; set; }

        public virtual UserIdentity Owner { get; set; }

        public string StackTrace { get; set; }

        public DateTime CompletedDate { get; set; }

        public virtual ICollection<TestPointFile> Files { get; set; } = new List<TestPointFile>();
    }

    public class TestPointFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
 }