using System;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Models.Suites;
using TestPlanService.Models.TestCases;
using TestPlanService.Services.Db.Tables;
using Navy.Core.Extensions;

namespace TestPlanService.Services.Db.SubSystems
{
    public class TestCasesSubsystem
    {
        DatabaseService _db;

        public TestCasesSubsystem(DatabaseService context)
        {
            _db = context;
        }

         
        public WorkItem AddTest(User user, Project project , AddOrUpdateTestCaseRequest request, bool isSave = true)
        { 
            var wi = new WorkItem()
            {
                CreatedBy = user,
                Project = project,
                Type = WiType.TestCase,
            };
            _db.Context.WorkItems.Add(wi);
            if(isSave)
                _db.Context.SaveChanges();
            UpdateTest(user, wi, request, isSave);
            return wi;
        }
         
        public void UpdateTest(User user, WorkItem wi, AddOrUpdateTestCaseRequest request, bool isSave = true)
        {   
            var rev = new WorkItemRevision()
            {
                AssignedTo = _db.Context.Users.FirstOrDefault(p => p.Id == request.AssignedTo),
                AutomationStatus = request.AutomationStatus,
                AutomationTestName = request.AutomationTestName,
                AutomationTestType = request.AutomationTestType,
                AutomationTestStorage = request.AutomationTestStorage,
                ChangeBy = user,
                Description = request.Description,
                Postcondition = request.Postcondition,
                Precondition = request.Precondition,
                Priority = request.Priority,
                State = request.State,
                Steps = request.Steps.XmlSerialize(),
                Title = request.Title, 

            };
            foreach (var s in wi.Steps.ToArray())
            {
                var newStep = request.Steps.FirstOrDefault(p => p.Id == s.Id);
                if (newStep == null)
                    _db.Context.TestSteps.Remove(s);
                else
                {
                    s.Order = newStep.Order;
                    s.Result = newStep.Result;
                    s.Action = newStep.Action;
                }
            }
            foreach (var newStep in request.Steps.Where(p => p.Id == null))
            {
                var s = new TestStep();
                s.Order = newStep.Order;
                s.Result = newStep.Result;
                s.Action = newStep.Action;
                wi.Steps.Add(s);
                _db.Context.TestSteps.Add(s);
            }
            wi.Revisions.Add(rev);
            _db.Context.WorkItemRevisions.Add(rev);
            if (isSave)
            {
                wi.Revision = rev;
                _db.Context.SaveChanges();
            }
        }


    }
}
