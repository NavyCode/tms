using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.ProjectUsers
{
    public class GetProjectUsersResponseItem
    {  
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; }
        public UserRole Role { get; set; }
    }
}
