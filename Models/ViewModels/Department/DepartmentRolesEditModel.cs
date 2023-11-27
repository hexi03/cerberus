using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cerberus.Models.ViewModels
{
    public class DepartmentRolesEditModel
    {

        [DepartmentID]
        public int id { get; set; }
        public Dictionary<string, string> Groups { get; set; }

        public DepartmentRolesEditModel() { }

        public List<string> getGroupIDs()
        {
            return Groups.Values.ToList();
        }

        public class Mapper : ITypeConverter<Department, DepartmentRolesEditModel>
        {
            public DepartmentRolesEditModel Convert(Department source, DepartmentRolesEditModel destination, ResolutionContext context)
            {
                destination = new DepartmentRolesEditModel();
                destination.id = source.id;
                destination.Groups = source.GroupDepartmentClaims.ToDictionary(e => Guid.NewGuid().ToString(), e => e.group_id);
                return destination;
            }
        }
    }
}