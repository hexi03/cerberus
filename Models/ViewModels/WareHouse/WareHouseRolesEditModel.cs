using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.edmx;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class WareHouseRolesEditModel
    {
        [Required]
        [WareHouseID]
        public int id { get; set; }
        public Dictionary<string, string> Roles { get; set; }

        public WareHouseRolesEditModel() { }

        public List<string> getGroupIDs()
        {
            return Roles.Values.ToList();
        }

        public class Mapper : ITypeConverter<Warehouse, WareHouseRolesEditModel>
        {
            public WareHouseRolesEditModel Convert(Warehouse source, WareHouseRolesEditModel destination, ResolutionContext context)
            {
                destination = new WareHouseRolesEditModel();
                destination.id = source.id;
                destination.Roles = source.GroupWareHouseClaims.ToDictionary(e => e.group_id, e => e.group_id);
                return destination;
            }
        }
    }
}