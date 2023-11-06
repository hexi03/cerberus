using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class FactorySiteRolesDTO
    {
        public FactorySite factorySite { get; set; }
        public int id { get; set; }
        public Dictionary<string, string> Roles { get; set; }

        public FactorySiteRolesDTO() { }
    }
}