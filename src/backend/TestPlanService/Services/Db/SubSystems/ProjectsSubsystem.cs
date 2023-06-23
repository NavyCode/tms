using System;
using System.Linq;
using TestPlanService.Models.Authorization;
using TestPlanService.Models.Projects;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Services.Db.SubSystems
{
    public class ProjectsSubsystem
    {
        DatabaseService _db;

        public ProjectsSubsystem(DatabaseService context)
        {
            _db = context;
        }

        public Project Find(int projectId)
        {
            return _db.Context.Projects.FirstOrDefault(p => p.Id == projectId);
        } 

        public Project AddProject(User owner, AddOrUpdateProjectRequest request)
        {
            var project = new Project()
            {
                Owner = owner,
            };
            _db.Context.Projects.Add(project);
            UpdateProject(project, request);
            _db.ProjectUsers.AddUser(project, owner, UserRole.Owner);
            return project;
        }

        public void UpdateProject(Project project, AddOrUpdateProjectRequest request)
        {
            project.Name = request.Name;
            project.Description = request.Description;
            _db.Context.SaveChanges();
        }
    }
}
