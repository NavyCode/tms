using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestRunsController : ControllerBase
    {
        private readonly ILogger<TestRunsController> _logger;
        DatabaseService _db;
        AccessService _access;

        public TestRunsController(ILogger<TestRunsController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpPost("runs")]
        public ActionResult<int> Add(int projectId, AddRunRequest request)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Projects.Find(projectId);
            var run = _db.Runs.AddRun(_access.User, project, request.IsAutomated, request.Name);
            return run.Id;
        }

        [HttpPost("runs/{runId}/results")]
        public ActionResult<int> AddResult(int projectId, int runId, TestResultRequest request)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.HasSession(Request);
            var testRun = _db.Context.TestRuns.FirstOrDefault(p => p.Id == runId);
            if (testRun == null || testRun.Project.Id != projectId)
                return NotFound();

            var result = new TestResult
            {
                AutomatedTestId = request.AutomatedTestId,
                AutomatedTestName = request.AutomatedTestName,
                AutomatedTestStorage = request.AutomatedTestStorage,
                CompletedDate = request.CompletedDate,
                ComputerName = request.ComputerName,
                ErrorMessage = request.ErrorMessage,
                OutCome = request.OutCome,
                Owner = _access.User,
                stackTrace = request.stackTrace,
                StartedDate = request.StartedDate,
                TestRun = testRun,
                Description = request.Description,
            };
            _db.Context.TestResults.Add(result);
            _db.Context.SaveChanges(); 
            return result.Id;
        }



        [HttpPost("runs/{runId}/results/{resultId}/files")] 
        public ActionResult AddFiles(int projectId, int resultId, int[] ids)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.HasSession(Request);
            var testResult = _db.Context.TestResults.FirstOrDefault(p => p.Id == resultId);
            if (testResult == null || testResult.TestRun?.Project.Id != projectId)
                return NotFound();

            foreach(var file in _db.Context.Files.Where(p => ids.Contains(p.Id)))
            {
                testResult.Files.Add(file);
            }
            _db.Context.SaveChanges();
            return Ok();
        }


        [HttpGet("runs/{runId}/results")]
        public ActionResult<List<TestRunPoint>> GetResults(int projectId, int runId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.HasSession(Request);
            var testRun = _db.Context.TestRuns.FirstOrDefault(p => p.Id == runId);
            if (testRun == null || testRun.Project.Id != projectId)
                return NotFound();

            var points = new List<TestRunPoint>();
            foreach (var request in testRun.Results)
            {
                var result = new TestRunPoint
                {
                    AutomatedTestId = request.AutomatedTestId,
                    AutomatedTestName = request.AutomatedTestName,
                    AutomatedTestStorage = request.AutomatedTestStorage,
                    CompletedDate = request.CompletedDate,
                    ComputerName = request.ComputerName,
                    ErrorMessage = request.ErrorMessage,
                    OutCome = request.OutCome,
                    StackTrace = request.stackTrace,
                    StartedDate = request.StartedDate,
                    Description = request.Description,
                    Id= request.Id,
                    Owner = new UserIdentity(testRun.Owner),
                    Files = request.Files.Select(p => new TestPointFile()
                    {
                        Id = p.Id,
                        Name = p.Name,
                    }).ToList()
                };
                points.Add(result);
            }
            return points;
        }
    }
}
