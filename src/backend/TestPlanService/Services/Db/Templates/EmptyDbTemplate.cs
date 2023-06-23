using System;
using System.Linq;
using System.Threading.Tasks;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Services.Db.Templates
{
    public class EmptyDbTemplate
    {
        public DatabaseService _db;

        public EmptyDbTemplate(DatabaseService db)
        {
            _db = db;
        }

        public void Fill()
        {
            if (!_db.Context.Users.Any(p => p.Login == "admin"))
            {
                var hash = Security.HashPasword("admin", out var salt);
                var user = _db.Users.AddRealUser(new Models.Authorization.RegisterOrUpdateUserRequest()
                {
                    Name = "Admin",
                    Login = "admin",
                    Pass = "admin",
                    Mail = "admin@localhost",
                });
                user.isRoot = true;
                _db.Context.SaveChanges();

                new DemoProjectTemplate(_db, _db._logger).CreateDemoProject(user);
            }
             
        }
    }
}
