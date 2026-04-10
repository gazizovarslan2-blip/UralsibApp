using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UralSibApp.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int? ManagerId { get; set; }

        public string DepartmentName { get; set; } = string.Empty;
        public string ManagerName { get; set; } = string.Empty;
        public int EmployeesCount { get; set; }
    }
}