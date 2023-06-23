using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Plans;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class PlansController : ControllerBase
    {
        private readonly ILogger<PlansController> _logger;
        DatabaseService _db;
        AccessService _access;

        public PlansController(ILogger<PlansController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpGet("plans")]
        public ActionResult<GetPlansResponse> GetPlans(int projectId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId); 
            if (_access.HasResult)
                return _access.Result;

            var result = new GetPlansResponse();
            foreach (var item in _db.Context.TestPlans.Where(p => p.Project.Id == projectId && !p.IsDeleted))
            {
                result.Plans.Add(TestPlanItem.FromDb(item));
            }
            return result;
        }

        [HttpPut("plans/{planId}")]
        public ActionResult UpdatePlan(int projectId, int planId, [FromBody] AddOrUpdatePlanRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var plan = _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);
            if (plan == null || plan.Project.Id != projectId)
                return NotFound();

            _db.Plans.UpdatePlan(plan, request);
            return Ok();
        }


        [HttpPost("plans/{planId}/dublicate")]
        public ActionResult<int> DublicatePlan(int projectId, int planId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result; 

            var plan = _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);
            if (plan == null || plan.Project.Id != projectId)
                return NotFound();

            _db.Plans.Dublicate(_access.User, plan);
            return Ok();
        }

        [HttpPost("plans")]
        public ActionResult<int> AddPlan(int projectId, AddOrUpdatePlanRequest request)
        { 
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var user = _access.User;
            var project = _db.Projects.Find(projectId);
            var plan = _db.Plans.AddPlan(user, project, request);
            return plan.Id;
        }
        
        [HttpDelete("plans/{planId}")]
        public ActionResult DeletePlan(int projectId, int planId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.TestManager });
            if (_access.HasResult)
                return _access.Result;

            var plan = _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);
            if (plan == null || plan.Project.Id != projectId)
                return NotFound();

            plan.Deactivate();
            _db.Context.SaveChanges();
            return Ok();
        }  

        [HttpGet("plan/{planId}")]
        public ActionResult<TestPlanItem> GetPlanInfo(int projectId, int planId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result; 

            var plan = _db.Context.TestPlans.FirstOrDefault(p => p.Id == planId);
            if (plan == null || plan.Project.Id != projectId)
                return NotFound();

            return TestPlanItem.FromDb(plan);
        }
    }
}
