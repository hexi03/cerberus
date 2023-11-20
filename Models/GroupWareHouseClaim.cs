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
    using static cerberus.Models.edmx.GroupDepartmentClaim;

    public partial class GroupWareHouseClaim
    {

        internal static IQueryable<Warehouse> get_group_warehouses(CerberusDBEntities context, ApplicationUserManager userManager, string user_id)
        {

            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));

            var group_names = userManager.GetRoles(user_id).ToList();
            var group_ids = userManager.GetRoles(user_id).ToList().Select(r => roleManager.FindByName(r).Id);


            if (group_names.Contains("Admin"))
            {
                    return context.WareHouses.Include(e => e.Department);
                }

                var deps = GroupDepartmentClaim.get_group_departments(context, userManager, user_id, Levels.Full).Select(e => e.id);

                var warehouses = context.GroupWareHouseClaims
                    .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.Warehouse.department_id)).Select(c => c.Warehouse).Include(e => e.Department);

                return warehouses;
            
        }
    }
}