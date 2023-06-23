using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Db.Templates;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Controllers
{
    [ApiController] 
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        DatabaseService _db;
        AccessService _access;

        public UsersController(ILogger<UsersController> logger, AccessService session, DatabaseService db)
        {
            _logger = logger;
            _db = db;
            _access = session;
        }

        [HttpPost("users/login")]
        public async Task<ActionResult<string>> Login(string login, string pass)
        {
            var session = await _access.Login(login, pass);
            if (session == null)
                return Unauthorized();

            Response?.Cookies?.Append("session", session);
            return session;
        } 

        [HttpGet("users/current/info")]
        public ActionResult<LoggedUserInfo> UserInfo()
        {
            _access.HasSession(Request);
            if (_access.HasResult)
                return _access.Result; 
            return _access.GetUserInfo(_access.User.Id);
        }

        [HttpPut("users/current/info")]
        public ActionResult UpdateCurrentUser(RegisterOrUpdateUserRequest request)
        {
            _access.HasSession(Request); 
            if (_access.HasResult)
                return _access.Result;

            _db.Users.UpdateUser(_access.User, request);
            return Ok();
        }
          
        [HttpPost("users")]
        public ActionResult<int> RegisterUser(RegisterOrUpdateUserRequest request)
        {
            var existUser = _db.Context.Users.FirstOrDefault(p => p.Login == request.Login);
            if (existUser != null)
                return Conflict($"User with login '{request.Login}' exist");

            var user = _db.Users.AddRealUser(request);
            new DemoProjectTemplate(_db, _logger).CreateDemoProject(user);
            return user.Id;
        }
    }
}
