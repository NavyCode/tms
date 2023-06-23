using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Runs;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestPointsController : ControllerBase
    {
        private readonly ILogger _logger;
        DatabaseService _db;
        AccessService _access;

        public TestPointsController(ILogger<TestPointsController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }


        [HttpGet("test/plans/{planId}/suites/{suiteId}/points")]
        public ActionResult<GetSuiteTestPointsResponse> GetSuiteTestPoints(int projectId, int suiteId, bool withCildrenSuites)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var dbSuite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (dbSuite == null || dbSuite.TestPlan.Project.Id != projectId)
                return NotFound();

            var defineCtrl = new PlanController(null, _access, _db);
            var suiteIds = _db.TestSuites.GetSuiteIds(dbSuite, withCildrenSuites);


            var lq = from point in _db.Context.TestPoints
                     join run in _db.Context.TestResults on point.RunResult equals run into pointRun
                     from r in pointRun.DefaultIfEmpty()
                     where point.SuiteTestCase.TestSuite.TestPlan.Id == dbSuite.TestPlan.Id
                     select new
                     {  
                         point.Id,
                         TestCaseId = point.SuiteTestCase.TestCase.Id,
                         TestSuiteId = point.SuiteTestCase.TestSuite.Id,
                         TestConfigId = point.TestConfig.Id,
                         Configuration = point.TestConfig.Name,
                         Name = point.SuiteTestCase.TestCase.Revision.Title,
                         point.SuiteTestCase.Order,
                         OutCome = r != null ? r.OutCome : TestPlanService.Outcome.Planed,
                         Description = r != null ? r.Description : null,
                         point.SuiteTestCase.TestCase.Revision.Priority,
                         Tester = point.Tester.Name,
                         PlanId = point.SuiteTestCase.TestSuite.TestPlan.Id
                     };



            var response = new GetSuiteTestPointsResponse();
            foreach (var point in lq)
            {
                if (suiteIds.Contains(point.TestSuiteId))
                {
                    response.Points.Add(new SuiteTestPointItem()
                    {
                        Id = point.Id,
                        TestCaseId = point.Id,
                        TestSuiteId = point.TestSuiteId,
                        TestConfigId = point.TestConfigId,
                        Configuration = point.Configuration,
                        Name = point.Name,
                        Order = point.Order,
                        Outcome = point.OutCome,
                        Priority = point.Priority,
                        Description = point.Description,
                        Tester = point.Tester
                    });
                }
            }
            return response;
        } 
        
        [HttpPost("test/plans/{planId}/testers")]
        public ActionResult SetTester(int projectId, SetTesterRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var user = _db.Context.Users.FirstOrDefault(p => p.Id == request.TesterId);
            foreach (var pointId in request.TestPointIds)
            {
                var point = _db.Context.TestPoints.FirstOrDefault(p => p.Id == pointId); 
                if (point == null || point.SuiteTestCase.TestSuite.TestPlan.Project.Id != projectId)
                    return NotFound(); 
                point.Tester = user;
            }
            _db.Context.SaveChanges();
            return Ok();
        }

        [HttpPost("test/plans/list/assignusers")]
        public ActionResult<List<UserIdentity>> GetUserListForAssign(int projectId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var result = new List<UserIdentity>();
            var project = _db.Projects.Find(projectId);
            foreach (var pu in project.Users.Where(p => !p.User.IsDeleted))
            {
                result.Add(new UserIdentity(pu.User));
            }
            return result;
        } 

        [HttpPost("test/plans/{planId}/suites/{suiteId}/tests/result")]
        public ActionResult SetOutcome(int projectId, SetOutcomeRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager, UserRole.Assessor });
            if (_access.HasResult)
                return _access.Result;
            var project = _db.Projects.Find(projectId);

            TestRun run = null;
            foreach (var pointId in request.TestPointIds)
            {
                var point = _db.Context.TestPoints.FirstOrDefault(p => p.Id == pointId);
                if (point == null || point.SuiteTestCase.TestSuite.TestPlan.Project != project)
                    return NotFound(); 
                if(point.RunResult == null)
                {
                    if(run == null)
                        run = _db.Runs.AddRun(_access.User, project, false, $"Test Plan '{point.SuiteTestCase.TestSuite.TestPlan.Title}'");
                    point.RunResult = _db.Runs.AddResult(_access.User, run, request.Outcome);
                }
                point.RunResult.OutCome = request.Outcome;
            }
            _db.Context.SaveChanges();
            return Ok();
        }

        TestRun run = null;
        [HttpPost("test/plans/{planId}/suites/{suiteId}/tests/comment")]
        public ActionResult SetComment(int projectId, SetDescriptionRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager, UserRole.Assessor });
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Projects.Find(projectId);
            TestRun run = null;
            foreach (var pointId in request.TestPointIds)
            {
                var point = _db.Context.TestPoints.FirstOrDefault(p => p.Id == pointId);
                if (point == null || point.SuiteTestCase.TestSuite.TestPlan.Project.Id != projectId)
                    return NotFound();

                if (point.RunResult == null)
                {
                    if (run == null)
                        run = _db.Runs.AddRun(_access.User, project, false, $"Test Plan '{point.SuiteTestCase.TestSuite.TestPlan.Title}'");
                    point.RunResult = _db.Runs.AddResult(_access.User, run, TestPlanService.Outcome.Planed);
                }
                point.RunResult.Description = request.Description;
            }
            _db.Context.SaveChanges();
            return Ok();
        }
    }
}
