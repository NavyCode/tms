using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.TestCases
{
    public class GetTestCaseResponse
    {
        public int Id { get; set; }
        public int Priority { get; set; }
        public WiState State { get; set; } 
        public virtual UserIdentity AssignedTo { get; set; } 
        public string Title { get; set; } 
        public string Description { get; set; } 
        public AutomationStatus AutomationStatus { get; set; }
        public string AutomationTestName { get; set; } 
        public string AutomationTestStorage { get; set; }  
        public string AutomationTestType { get; set; }
        public string Precondition { get; set; }
        public string Postcondition { get; set; }
        public virtual UserIdentity ChangeBy { get; set; }
        public List<TestStepInfo> Steps { get; set; } = new List<TestStepInfo>();
    }


    public class TestStepInfo
    {
        public int Id { get; set; }

        public int Order { get; set; }

        public string Action { get; set; }

        public string Result { get; set; }
    } 
}
