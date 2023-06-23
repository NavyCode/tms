using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Authorization
{
    public class UserIdentity
    {
        public UserIdentity(User assignedTo)
        {
            this.Id = assignedTo.Id;
            this.Name = assignedTo.Name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
