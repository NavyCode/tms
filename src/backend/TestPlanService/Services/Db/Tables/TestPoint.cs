using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class TestPoint
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public virtual TestConfig TestConfig { get; set; }

        public virtual User Tester { get; set; }

        public virtual SuiteTestCase SuiteTestCase { get; set; }

        public virtual TestResult RunResult { get; set; }

    }
}