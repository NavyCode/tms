using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.ProjectUsers
{
    public class AddOrUpdateExistUserRequest
    { 
        public string Mail { get; set; } 
        public UserRole Role { get; set; }
    }
}
