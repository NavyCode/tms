using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.TestCases;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using Navy.Core.Extensions;
using System.Collections.Generic;
using TestPlanService.Models.TestCase;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TestPlanService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestCasesController : ControllerBase
    {
        private readonly ILogger<TestCasesController> _logger;
        DatabaseService _db;
        AccessService _access;

        public TestCasesController(ILogger<TestCasesController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpGet("tests/{testId}")]
        public ActionResult<GetTestCaseResponse> GetTestCase(int projectId, int testId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.HasSession(Request);
            var wi = _db.Context.WorkItems.FirstOrDefault(p => p.Id == testId && !p.IsDeleted);
            if (wi == null || wi.Project.Id != projectId)
                return NotFound();

            var rev = wi.Revision;
            var result = new GetTestCaseResponse()
            {
                Id = wi.Id,
                AssignedTo = new UserIdentity(rev.AssignedTo),
                AutomationStatus = rev.AutomationStatus,
                AutomationTestName = rev.AutomationTestName,
                AutomationTestStorage = rev.AutomationTestStorage,
                AutomationTestType = rev.AutomationTestType,
                ChangeBy = new UserIdentity(rev.ChangeBy),
                Description = rev.Description,
                Postcondition = rev.Postcondition,
                Precondition = rev.Precondition,
                Priority = rev.Priority,
                State = rev.State,
                Title = rev.Title
            };
            foreach(var s in wi.Steps)
            {
                result.Steps.Add(new TestStepInfo()
                {
                    Action = s.Action,
                    Id = s.Id,
                    Order = s.Order,
                    Result = s.Result
                });
            }
            return result;
        }
         
        [HttpGet("tests/search")]
        public ActionResult<List<SearchTestCaseItem>> SearchTestCases(int projectId, string text)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId); 
            if (_access.HasResult)
                return _access.Result;

            text = text ?? "";
            var result = new List<SearchTestCaseItem>();
            if (int.TryParse(text, out var id))
            {
                var testById = _db.Context.WorkItems.FirstOrDefault(p => p.Project.Id == projectId && p.Id == id);
                if (testById != null)
                    result.Add(SearchTestCaseItem.FromDb(testById));
            } 
            foreach (var t in _db.Context.WorkItems.Where(p => p.Project.Id == projectId 
                && p.Revision.Title.ToLower().Contains(text.ToLower()))
                .OrderBy(p => p.Id)
                .Take(100))
            {
                if(t.Id != id)
                    result.Add(SearchTestCaseItem.FromDb(t));
            }
            return result;
        }


        [HttpPost("tests")]
        public ActionResult<int> AddTest(int projectId, [FromBody] AddOrUpdateTestCaseRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;
             
            var project = _db.Projects.Find(projectId);
            var wi = _db.TestCases.AddTest(_access.User, project, request);
            return wi.Id;
        }

        [HttpPut("tests/{testId}")]
        public ActionResult UpdateTest(int projectId, int testId, [FromBody] AddOrUpdateTestCaseRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var wi = _db.Context.WorkItems.FirstOrDefault(p => p.Id == testId);
            if (wi == null || wi.Project.Id != projectId)
                return NotFound();
            _db.TestCases.UpdateTest(_access.User, wi, request);
            return Ok();
        }

        [HttpDelete("tests/{testId}")]
        public ActionResult DeleteTest(int projectId, int testId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var wi = _db.Context.WorkItems.FirstOrDefault(p => p.Id == testId);
            if (wi == null || wi.Project.Id != projectId)
                return NotFound();
            wi?.Deactivate();
            _db.Context.SaveChanges();
            return Ok();
        }
    }
}
