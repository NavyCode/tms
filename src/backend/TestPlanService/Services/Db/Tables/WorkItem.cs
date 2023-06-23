using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class WorkItem : TableBase
    {
        public virtual User CreatedBy { get; set; }

        public virtual WorkItemRevision Revision { get; set; }

        public WiType Type { get; set; } 

        public virtual Project Project { get; set; }

        public virtual ICollection<TestStep> Steps { get; set; } = new List<TestStep>();

        public virtual ICollection<WorkItemRevision> Revisions { get; set; } = new List<WorkItemRevision>();
    }

    public class WorkItemRevision
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public WiState State { get; set; }
         
        public virtual User AssignedTo { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
        public string Precondition { get; set; } 
        public string Postcondition { get; set; } 

        public AutomationStatus AutomationStatus { get; set; }
        public int Priority { get; set; }

        public string AutomationTestName { get; set; }

        public string AutomationTestStorage { get; set; }

        public string AutomationTestType { get; set; } 
        public virtual User ChangeBy { get; set; }

        public string Steps { get; set; }
    }
}