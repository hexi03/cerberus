using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Drawing;

    public partial class GroupDepartmentClaim
    {
        public enum Levels{
            Full,
            Partial
        }
        public static IQueryable<Department> get_group_departments(CerberusDBEntities context, ApplicationUserManager userManager, string user_id, Levels level)
        {
            
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
            
            var group_names = userManager.GetRoles(user_id).ToList();
            var group_ids = userManager.GetRoles(user_id).ToList().Select(r => roleManager.FindByName(r).Id);
            

            if (group_names.Contains("Admin"))
                {
                    return context.Departments;
                }
                
                switch (level) {
                    

                    case Levels.Full:
                        return context.GroupDepartmentClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department);
                    case Levels.Partial:
                        var deps = context.GroupFactorySiteClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.FactorySite.Department)
                            .Union(context.GroupWareHouseClaims
                                .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Warehouse.Department));
                        return context.GroupDepartmentClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department).Union(deps);
                default:
                        throw new Exception();
                }

            
        }
    }
}