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
    public class PlanController : ControllerBase
    {
        private readonly ILogger<PlanController> _logger;
        DatabaseService _db;
        AccessService _access;

        public PlanController(ILogger<PlanController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpGet("test/plans/{planId}/suites/tree")]
        public ActionResult<GetSuitesTreeResponse> GetSuitesTree(int projectId, int planId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var plan = _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);
            if (plan == null || plan.Project.Id != projectId)
                return Unauthorized();

            var response = new GetSuitesTreeResponse();
            foreach (var dbSuite in plan.Suites)
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
          
        

    }
}
