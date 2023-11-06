using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class DepartmentRolesDTO
    {
        public Department department {  get; set; }
        public int id { get; set; }
        public Dictionary<string, string> Roles { get; set; }

        public DepartmentRolesDTO() { }
    }
}