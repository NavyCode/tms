using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Db.Templates;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db;
using Microsoft.AspNetCore.Mvc;

namespace TestPlanService.Services.Sessions
{
    public class AccessService
    {
        public bool HasResult => Result != null;

        public ActionResult Result { get; private set; }
        public User User { get; set; } = null;

        private ILogger _logger;
        private DatabaseService _db;

        public AccessService(ILogger<AccessService> logger, DatabaseService db)
        {
            _logger = logger;
            _db = db;
        }

        static TimeSpan SessionTimeOut = TimeSpan.FromDays(30);

        public AccessService HasSession(HttpRequest request)
        {
            var session = request.Cookies["session"];
            if (session == null)
            {
                if (request.Headers.TryGetValue("session", out var sh))
                    session = sh;
                else
                {
                    _logger.LogInformation("no session key");
                    Result = new UnauthorizedResult();
                    return this;
                }
            }
            var dbsession = _db.Context.Sessions.FirstOrDefault(p => p.Key == session);
            if (dbsession == null)
            {
                _logger.LogInformation($"Session '{session}' not found");
                Result = new UnauthorizedResult();
                return this;
            }
            if (DateTime.UtcNow - dbsession.LastTime > SessionTimeOut)
            {
                _logger.LogInformation($"Session '{session}' outdate");
                Result = new UnauthorizedResult();
                return this;
            }
            if (DateTime.UtcNow - dbsession.LastTime > TimeSpan.FromDays(1))
            {
                dbsession.LastTime = DateTime.UtcNow;
                _db.Context.SaveChanges();
            }
            User = dbsession.User;
            return this;
        }

        internal async Task<string> Login(string login, string pass)
        { 
            var user = await _db.Context.Users.FirstOrDefaultAsync(p => p.Login == login);
            if (user != null)
            {
                if(Security.VerifyPassword(pass, user.Hash, user.Salt))
                    return _db.Users.CreateSession(user);
            }
            return null;
        }



        public LoggedUserInfo GetUserInfo(int userId)
        { 
            var user = _db.Context.Users.First(p => p.Id == userId);
            return new LoggedUserInfo()
            {
                Id = user.Id,
                Name = user.Name,
                IsRoot = user.isRoot
            };
        }


        public AccessService HasAnyUserRole(int project) => HasUserRole(project, null);

        public AccessService HasUserRole(int project, UserRole[] roles)
        {
            if (HasResult)
                return this;
            var pu = _db.Projects.Find(project)?
                .Users.FirstOrDefault(p => p.User == User);
            if (pu == null)
            {
                Result = new UnauthorizedResult();
                return this;
            }
            if (pu.Role != UserRole.Owner)
            {
                if (roles != null && !roles.Contains(pu.Role))
                {
                    Result = new UnauthorizedResult();
                    return this;
                }
            }
            return this;
        }

        public AccessService RealUser()
        {
            if (HasResult)
                return this;
            if (User.isVirtual)
                Result = new UnauthorizedResult();
            return this;
        }
    }
}