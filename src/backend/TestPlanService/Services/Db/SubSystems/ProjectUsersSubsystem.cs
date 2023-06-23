using System;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Models.ProjectUsers;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class ProjectUsersSubsystem
    {
        DatabaseService _db;

        public ProjectUsersSubsystem(DatabaseService context)
        {
            _db = context;
        }
        public ProjectUser AddUser(Project project, User user, UserRole role)
        {
            var projectUser = project.Users.FirstOrDefault(p => p.User == user);
            if (projectUser != null)
                return projectUser;

            projectUser = new ProjectUser()
            {
                Project = project,
                User = user
            };
            project.Users.Add(projectUser);
            _db.Context.ProjectUsers.Add(projectUser);
            _db.Context.SaveChanges();

            UpdateExistUser(projectUser, role);
            return projectUser;
        }
         

        public void UpdateExistUser(ProjectUser user, UserRole role)
        {
            user.Role = role;
            _db.Context.SaveChanges();
        }
    }
}
