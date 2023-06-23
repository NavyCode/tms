using System.Collections.Generic;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService.Models.Authorization
{
    public class GetUsersResponse
    {
        public List<UserItem> Users { get; set; } = new List<UserItem>();
    }

    public class UserItem
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Mail { get; set; }
        public UserRole Role { get; set; }
        public bool IsVirtual { get; set; }
        public UserIdentity Identity { get; internal set; }
    }
}
