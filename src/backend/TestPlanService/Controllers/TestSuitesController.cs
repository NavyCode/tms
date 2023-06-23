using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestSuitesController : ControllerBase
    {
        private readonly ILogger _logger;
        DatabaseService _db;
        AccessService _access;

        public TestSuitesController(ILogger<TestSuitesController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }
        [HttpGet("test/plans/{planId}/suites/{suiteId}/children")]
        public ActionResult<GetSuitesTreeResponse> GetChildrenSuites(int projectId, int suiteId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;
            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId); 
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();

            var response = new GetSuitesTreeResponse();
            foreach (var dbSuite in _db.Context.TestSuites.Where(p => p.Parent.Id == suiteId))
            {
                response.Suites.Add(new SuitesTreeItem()
                {
                    Id = dbSuite.Id,
                    Name = dbSuite.Title,
                    ParentId = dbSuite.Parent?.Id
                });
            }
            return response;
        }

        [HttpPost("test/plans/{planId}/suites")]
        public ActionResult<int> AddSuite(int projectId, int planId, AddOrUpdateSuiteRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var plan = _db.Plans.Find(planId);
            if (plan == null || plan.Project.Id != projectId)
                return NotFound();

            var parent = _db.Context.TestSuites.FirstOrDefault(p => p.Id == request.ParentId);
            var suite = _db.TestSuites.AddSuite(plan, parent, request);
            return suite.Id;
        }

        [HttpPut("test/plans/{planId}/suites/{suiteId}")]
        public ActionResult UpdateSuite(int projectId, int suiteId, AddOrUpdateSuiteRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();

            _db.TestSuites.UpdateSuite(suite, request);
            return Ok();
        } 

        [HttpDelete("test/plans/{planId}/suites/{suiteId}")]
        public ActionResult DeleteSuite(int projectId, int suiteId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();

            _db.Context.TestSuites.Remove(suite);
            _db.Context.SaveChanges();
            return Ok();
        }

        public ActionResult MoveSuite(int projectId, int id, MoveSuiteRequest moveSuiteRequest)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == id);
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();

            var destination = _db.Context.TestSuites.FirstOrDefault(p => p.Id == moveSuiteRequest.NewParentId); 
            if (destination == null || destination.TestPlan.Project.Id != projectId)
                return NotFound();

            var p = destination;
            while(p != null)
            {
                if (p.Id == suite.Id)
                    return Conflict($"Desctination suite {destination.Id} is child of current suite {suite.Id}");
                p = p.Parent;
            }

            suite.Parent = destination;
            _db.Context.SaveChanges();
            return Ok();
        }
    }
}
