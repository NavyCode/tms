using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Authorization
{
    public class LoggedUserInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRoot { get; set; }
    }
}
