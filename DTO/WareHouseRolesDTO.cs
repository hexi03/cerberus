using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace cerberus.DTO
{
    public class WareHouseRolesDTO
    {
        public Warehouse warehouse;
        [Required]
        public int id { get; set; }
        public Dictionary<string,string> Roles { get; set; }

        public WareHouseRolesDTO() { }
    }
}