using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using TestPlanService.Services.Db.Tables;

namespace TestPlanService
{
    public class ProjectUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Project Project { get; set; }

        public virtual User User { get; set; }

        public UserRole Role { get; set; }
    }
}