using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.ProjectUsers;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Controllers
{
    [ApiController]
    [Route("projects/{projectId}")]
    public class ProjectUsersController : ControllerBase
    {
        private readonly ILogger<ProjectUsersController> _logger;
        DatabaseService _db;
        AccessService _access;

        public ProjectUsersController(ILogger<ProjectUsersController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        } 

        [HttpDelete("users/{id}")]
        public ActionResult DeleteUser(int projectId, int id)
        { 
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result; 

            var pu = _db.Context.ProjectUsers.FirstOrDefault(p => p.Id == id);

            if (pu == null || pu.Project.Id != projectId)
                return NotFound();

            _db.Context.ProjectUsers.Remove(pu);
            _db.Context.SaveChanges();
            return Ok();
        }

        [HttpGet("users")]
        public ActionResult<GetUsersResponse> GetUsers(int projectId)
        { 
            _access.HasSession(Request).HasAnyUserRole(projectId);
            if (_access.HasResult)
                return _access.Result;

            var user = _access.User;
            var result = new GetUsersResponse();
            var project = _db.Projects.Find(projectId);
            foreach (var u in project.Users)
            {
                var userItem = new UserItem()
                {
                    Id = u.Id,
                    Identity = new UserIdentity(u.User),
                    Mail = u.User.Mail,
                    IsVirtual = u.User.isVirtual,
                    Role = u.Role
                };
                if (u.User.isVirtual || u.User == user)
                {
                    userItem.Login = u.User.Login;
                    userItem.Name = u.User.Name;
                }
                result.Users.Add(userItem);
            }
            return result;
        }

        [HttpPost("users")]
        public ActionResult<int> AddVirtualUser(int projectId, AddOrUpdateVirtualUserRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result;
             
            var user = new User()
            {
                isVirtual = true,
            };
            _db.Context.Users.Add(user);
            var project = _db.Context.Projects.FirstOrDefault(p => p.Id == projectId);
            var projectUser = _db.ProjectUsers.AddUser(project, user, request.Role);
            _db.Context.SaveChanges();
            UpdateVirtualUser(projectId, projectUser.Id, request);
            
            return projectUser.Id;
        }


        [HttpPut("users/{userId}/info")]
        public ActionResult UpdateVirtualUser(int projectId, int userId, AddOrUpdateVirtualUserRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result; 

            var user = _db.Context.ProjectUsers.FirstOrDefault(p => p.Id == userId);
            if (user == null || !user.User.isVirtual)
                return NotFound();
            if (user.Project.Id != projectId)
                return NotFound();

            user.Role = request.Role;
            user.User.Login = request.Login;
            user.User.Mail = request.Mail;
            user.User.Name = request.Name;
            user.User.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Pass))
            {
                var hash = Security.HashPasword(request.Pass, out var salt);
                user.User.Salt = salt;
                user.User.Hash = hash;
            }
            _db.Context.SaveChanges();

            return Ok();
        }
         



        [HttpPost("users/exist")]
        public ActionResult<int?> AddExistUser(int projectId, AddOrUpdateExistUserRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result;

            var currentUser = _access.HasSession(Request);
            var project = _db.Context.Projects.FirstOrDefault(p => p.Id == projectId);
            var user = _db.Context.Users.FirstOrDefault(p => p.Mail.ToLower() == request.Mail.ToLower());
            if (user == null)
                return NotFound(); 

            var projectUser = _db.ProjectUsers.AddUser(project, user, request.Role);

            return projectUser.Id;
        }

        [HttpPut("users/exist/{userId}")]
        public ActionResult UpdateExistUser(int projectId, int userId, AddOrUpdateExistUserRequest request)
        {
            _access.HasSession(Request).HasUserRole(projectId, new[] { UserRole.Owner });
            if (_access.HasResult)
                return _access.Result;

            var user = _db.Context.ProjectUsers.FirstOrDefault(p => p.Id == userId);
            if (user == null || user.Project.Id != projectId)
                return NotFound();

            _db.ProjectUsers.UpdateExistUser(user, request.Role); 
            _db.Context.SaveChanges();
            return Ok();
        }
    }
}
