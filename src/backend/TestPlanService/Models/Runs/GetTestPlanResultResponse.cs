using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{
    public class GetTestPlanResultResponse
    {
        public int TestPlanId { get; set; }

        public string TestPlanName { get; set; }

        public List<TestPlanResultTestSuite> TestSuites { get; set; }
        public List<TestPlanResultTestCase> TestCases { get; set; }
    }

    public class TestPlanResultTestSuite
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Blocked { get; set; }
        public int Skipped { get; set; }
        public int Unknow { get; set; }
    }

    public class TestPlanResultTestCase
    {
        public int PointId { get; set; }
        public int TestCaseId { get; set; }
        public int TestSuitId { get; set; }
        public string Name{ get; set; }
        public int Order { get; set; }
        public Outcome Outcome { get; set; }
        public string Config { get; set; }
        public string Tester { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string Comment { get; set; }
    }
}
