using System;
using System.Collections.Generic;

namespace TestPlanService.Services.Db.Tables
{
    public class Project : TableBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual User Owner { get; set; } 

        public virtual ICollection<ProjectUser> Users { get; set; } = new List<ProjectUser>();
    } 
}