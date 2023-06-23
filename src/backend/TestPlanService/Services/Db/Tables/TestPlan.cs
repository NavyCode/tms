using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class TestPlan : TableBase
    {
        public virtual ICollection<TestSuite> Suites { get; set; } = new List<TestSuite>(); 
        public virtual Project Project { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public PlanState State { get; set; } 
        public virtual User CreatedBy { get; set; }
        public virtual User AssignedTo { get; set; }
    }

    public class TestSuite
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual TestPlan TestPlan { get; set; }

        public virtual TestSuite Parent { get; set; }

        public virtual ICollection<SuiteTestCase> TestCases { get; set; } = new List<SuiteTestCase>();

        public string Title { get; set; }
        public virtual ICollection<TestSuiteConfig> Configs { get; set; } = new List<TestSuiteConfig>();

    }

    public class TestSuiteConfig
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual TestSuite Suite { get; set; }

        public virtual TestConfig TestConfig { get; set; }
    }
}