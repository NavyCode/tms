using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class TestRun
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool IsAutomated { get; set; }

        public virtual User Owner { get; set; }

        public DateTime StartDate { get; set; }

        public virtual ICollection<TestResult> Results { get; set; } = new List<TestResult>();

        public virtual Project Project { get; set; }
        public string Name { get;  set; }
    }

    public class File
    { 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public byte[] Data { get; set; }

        public DateTime CreateDate { get; set; }

        public virtual Project Project { get; set; }

        public string Tags { get; set; }

        public string Name { get; set; }

        public bool Compressed { get; set; }
    }
}