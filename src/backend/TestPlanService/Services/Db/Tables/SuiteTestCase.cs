using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class SuiteTestCase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual WorkItem TestCase { get; set; }
        public int Order { get; set; }
        public virtual List<TestPoint> Points { get; set; } = new List<TestPoint>();

        public virtual TestSuite TestSuite { get; set; }

    }
}