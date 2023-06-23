using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Export.Navy.V1;
using TestPlanService.Models.Suites;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;
using Navy.Core.Extensions;
using System.IO;
using static Azure.Core.HttpHeader;
using TestPlanService.Services.Db.SubSystems.TestSuites;

namespace TestSuiteService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class TestSuiteController : ControllerBase
    {
        private readonly ILogger _logger;
        DatabaseService _db;
        AccessService _access;

        public TestSuiteController(ILogger<TestSuiteController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        } 

        [HttpGet("test/plans/{planId}/suites/{suiteId}/tests")]
        public ActionResult<GetSuiteTestCasesResponse> GetSuiteTestCases(int projectId, int suiteId, bool withCildren)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var dbSuite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (dbSuite == null || dbSuite.TestPlan.Project.Id != projectId)
                return NotFound();

            HashSet<int> suiteIds = _db.TestSuites.GetSuiteIds(dbSuite, withCildren);

            var response = new GetSuiteTestCasesResponse();
            foreach (var tc in _db.Context.SuiteTestCases
                .Select(tc => new
                {
                    tc.Id,
                    PlanId = tc.TestSuite.TestPlan.Id,
                    TestSuiteId = tc.TestSuite.Id,
                    TestCaseId = tc.TestCase.Id,
                    Name = tc.TestCase.Revision.Title,
                    tc.Order,
                    tc.TestCase.Revision.Priority,
                    tc.TestCase.Revision.State
                })
                .Where(p => p.PlanId == dbSuite.TestPlan.Id))
            {
                if (suiteIds.Contains(tc.TestSuiteId))
                {
                    response.Tests.Add(new SuiteTestCaseItem()
                    {
                        Id = tc.Id,
                        TestCaseId = tc.TestCaseId,
                        Name = tc.Name,
                        Order = tc.Order,
                        Priority = tc.Priority,
                        State = tc.State
                    });
                }
            }
            return response;
        }


        [HttpGet("test/plans/{planId}/suites/{suiteId}/configs")]
        public ActionResult<int[]> GetSuiteConfigs(int projectId, int suiteId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var dbSuite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId); 
            if (dbSuite == null || dbSuite.TestPlan.Project.Id != projectId)
                return NotFound();

            return dbSuite.Configs.Select(p => p.TestConfig.Id).ToArray();
        }

        [HttpPost("test/plans/{planId}/suites/{suiteId}/tests")]
        public ActionResult AddTestCaseSuite(int projectId, int suiteId, AddTestCaseToSuiteRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var user = _access.HasSession(Request);
            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();
            _db.TestSuite.AddExistTestCase(_access.User, suite, request);
            _db.Context.SaveChanges();
            return Ok();
        }

        [HttpDelete("test/plans/{planId}/suites/{suiteId}/tests")]
        public ActionResult DeleteTestCaseFromSuite(int projectId, DeleteTestCaseFromSuiteRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            foreach (var tc in request.SuiteTestCasesIds)
            {
                var stc = _db.Context.SuiteTestCases.FirstOrDefault(p => p.Id == tc);
                if (stc == null || stc.TestSuite.TestPlan.Project.Id != projectId)
                    continue;
                _db.Context.TestPoints.RemoveRange(stc.Points);
                _db.Context.SuiteTestCases.Remove(stc);
            } 
            _db.Context.SaveChanges();
            return Ok();
        }

        [HttpPost("test/plans/{planId}/suites/{suiteId}/tests/reorder")]
        public ActionResult ReorderTestCasess(int projectId, ReorderTestCasesRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            foreach (var kvp in request.SuiteTestCaseOrderDictionary)
            {
                var stc = _db.Context.SuiteTestCases.First(p => p.Id == kvp.Key);
                if (stc == null || stc.TestSuite.TestPlan.Project.Id != projectId)
                    continue;
                stc.Order = kvp.Value;
            }
            _db.Context.SaveChanges();
            return Ok();
        }


        [HttpPost("test/plans/{planId}/suites/{suiteId}/testconfigs")]
        public ActionResult SetTestCaseConfig(int projectId, SetTestCaseConfigRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            _db.TestSuite.SetSuiteTestCaseConfig(_access.User, projectId, request); 
            return Ok();
        }

        [HttpPost("test/plans/list/configs")]
        public ActionResult<GetConfigsForAssignResponse> GetConfigsForAssign(int projectId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var result = new GetConfigsForAssignResponse();
            foreach (var config in _db.Context.TestConfigs.Where(p => p.Project.Id == projectId && !p.IsDeleted))
            {
                result.Configs.Add(new ConfigsForAssign()
                {
                    Id = config.Id,
                    Name = config.Name,
                    Values = string.Join(", ", config.Params.Select(p => $"{p.Variable.Name}: {p.VariableParam.Value}"))
                });
            }
            return result;
        }
         

        [HttpPut("test/plans/{planId}/suites/{suiteId}/configs")]
        public ActionResult SetSuiteConfig(int projectId, int suiteId, SetTestSuiteConfigRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var suite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (suite == null || suite.TestPlan.Project.Id != projectId)
                return NotFound();

            _db.TestSuite.SetSuiteConfig(suite, request);
            return Ok();
        }

        [HttpGet("test/plans/{planId}/suites/{suiteId}/export")]
        public FileContentResult Export(int projectId, int suiteId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return null; 

            var dbSuite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (dbSuite == null || dbSuite.TestPlan.Project.Id != projectId)
                return null;
            var result = new SuiteExporter(_db).Create(dbSuite);

            return File(result, "text/xml", "suite.xml");
        }


        [HttpPost("test/plans/{planId}/suites/{suiteId}/import")]
        public async Task<ActionResult> Import(int projectId, int suiteId, ImportType type, byte[] data)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Tester, UserRole.TestManager });
            if (_access.HasResult)
                return null;

            var dbSuite = _db.Context.TestSuites.FirstOrDefault(p => p.Id == suiteId);
            if (dbSuite == null || dbSuite.TestPlan.Project.Id != projectId)
                return null;
            var imporeter = new SuiteImporter(_db, _logger);
            await imporeter.Import(data, type, dbSuite, _access.User);
            return Ok();
        }

    }
}
