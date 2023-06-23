using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.ProjectUsers
{
    public class AddOrUpdateVirtualUserRequest
    { 
        public string Name { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; } 
        public UserRole Role { get; set; }
    }
}
