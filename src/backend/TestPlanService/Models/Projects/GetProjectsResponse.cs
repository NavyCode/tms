using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Projects
{
    public class GetProjectsResponse
    {
        public List<ProjectItem> Projects { get; set; } = new List<ProjectItem>();
    }

    public class ProjectItem
    { 

        public static ProjectItem FromDb(Project project)
        {
            return new ProjectItem()
            {
                Id = project.Id,
                Comment = project.Description,
                Name = project.Name,
            };
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
    }
}
