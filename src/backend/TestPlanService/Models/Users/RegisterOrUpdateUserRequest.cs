using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Authorization
{
    public class RegisterOrUpdateUserRequest
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; } 
        public string Mail { get; set; }
    }
}
