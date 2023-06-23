using System;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Plans;
using TestPlanService.Models.Projects;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class AccessSubsystem
    {
        DatabaseService _db;

        public AccessSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public bool HasUserRole(int project, User user, UserRole[] roles = null)
        {
            var pu = _db.Projects.Find(project)?
                .Users.FirstOrDefault(p => p.User == user);
            if (pu == null)
                return false;
            if (roles == null)
                return true;
            return roles.Contains(pu.Role);
        }
    }
}
