using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Namotion.Reflection;
using System.Linq;
using TestPlanService.Models.Projects;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Controllers
{
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly ILogger<ProjectsController> _logger;
        DatabaseService _db;
        AccessService _access;

        public ProjectsController(ILogger<ProjectsController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpGet("projects")]
        public ActionResult<GetProjectsResponse> GetProjects()
        {
            _access.HasSession(Request);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.User;

            var result = new GetProjectsResponse();
            foreach (var pu in _db.Context.ProjectUsers.Where(p => p.User == user))
            {
                var project = pu.Project;
                if (project.IsDeleted)
                    continue;
                result.Projects.Add(ProjectItem.FromDb(project));
            }
            return result;
        }

        [HttpGet("project/{projectId}")]
        public ActionResult<ProjectItem> GetProjectInfo(int projectId)
        {
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Context.Projects.Find(projectId);
            return ProjectItem.FromDb(project);
        }

        [HttpPut("project/{projectId}")]
        public ActionResult UpdateProject(int projectId, [FromBody] AddOrUpdateProjectRequest request)
        { 
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result;

            var project = _db.Context.Projects.Find(projectId);
            _db.Projects.UpdateProject(project, request);
            return Ok();
        }

        [HttpPost("projects")]
        public ActionResult<int> AddProject(AddOrUpdateProjectRequest request)
        {
            _access.HasSession(Request).RealUser();
            if (_access.HasResult)
                return _access.Result;

            var user = _access.User;
            return _db.Projects.AddProject(user, request).Id;
        }

        [HttpDelete("projects/{projectId}")] 
        public ActionResult DeleteProject(int projectId)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result;
             
            var project = _db.Context.Projects.FirstOrDefault(p => p.Id == projectId);
            project?.Deactivate();
            _db.Context.SaveChanges();

            return Ok();
        }
    }
}
