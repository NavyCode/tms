using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestPlanService.Services.Db.Tables
{
    public class User : TableBase
    {
        public byte[] Salt { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Phone { get; set; }
        public string Mail { get; set; } 
        public bool isVirtual { get; set; }

        public bool isRoot { get; set; }
    }


    public enum UserRole
    {
        Guest = 0,
        Tester = 1,
        TestManager = 2,
        Owner = 3,
        Assessor = 4,
    }
}
