using System;
using TestPlanService.Models.Authorization;
using TestPlanService.Services.Db.Tables;
using TestPlanService.Services.Sessions;

namespace TestPlanService.Services.Db.SubSystems
{
    public class UsersSubsystem
    {
        DatabaseService _db;

        public UsersSubsystem(DatabaseService context)
        {
            _db = context;
        }  
        public void UpdateUser(User user, RegisterOrUpdateUserRequest request)
        {
            user.Login = request.Login;
            user.Mail = request.Mail;
            user.Name = request.Name;
            if (!string.IsNullOrEmpty(request.Pass))
            {
                var hash = Security.HashPasword(request.Pass, out var salt);
                user.Salt = salt;
                user.Hash = hash;
            }
            _db.Context.SaveChanges();
        } 
         
        public User AddRealUser(RegisterOrUpdateUserRequest request)
        {
            var user = new User();
            _db.Context.Users.Add(user);
            _db.Context.SaveChanges();
            UpdateUser(user, request);
            return user;
        }

        internal string CreateSession(User user)
        {
            var key = Guid.NewGuid().ToString();
            _db.Context.Sessions.Add(new Session()
            {
                Key = key,
                LastTime = DateTime.UtcNow,
                Tags = null,
                User = user
            });
            _db.Context.SaveChanges();
            return key;
        }
    }
}
