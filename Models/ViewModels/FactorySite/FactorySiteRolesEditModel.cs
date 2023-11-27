using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.edmx;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class FactorySiteRolesEditModel
    {
        [Required]
        [FactorySiteID]
        public int id { get; set; }
        //[RoleStringList]
        public Dictionary<string, string> Roles { get; set; }

        public FactorySiteRolesEditModel() { }

        public List<string> getGroupIDs()
        {
            return Roles.Values.ToList();
        }

        public class Mapper : ITypeConverter<FactorySite, FactorySiteRolesEditModel>
        {
            public FactorySiteRolesEditModel Convert(FactorySite source, FactorySiteRolesEditModel destination, ResolutionContext context)
            {
                destination = new FactorySiteRolesEditModel();
                destination.id = source.id;
                destination.Roles = source.GroupFactorySiteClaims.ToDictionary(e => e.group_id, e => e.group_id);
                return destination;
            }
        }
    }
}