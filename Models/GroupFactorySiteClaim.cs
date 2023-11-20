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

    public partial class GroupFactorySiteClaim
    {
        internal static IQueryable<FactorySite> get_group_factorysites(CerberusDBEntities context, ApplicationUserManager userManager, string user_id)
        {

            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));

            var group_names = userManager.GetRoles(user_id).ToList();
            var group_ids = userManager.GetRoles(user_id).ToList().Select(r => roleManager.FindByName(r).Id);


            if (group_names.Contains("Admin"))
            {
                    return context.FactorySites.Include(e => e.Department);
                }
                var deps = GroupDepartmentClaim.get_group_departments(context, userManager, user_id, Levels.Full).Select(e => e.id);
                var factorysites = context.GroupFactorySiteClaims
                        .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.FactorySite.department_id)).Select(c => c.FactorySite).Include(e => e.Department);
                return factorysites;
            
        }
    }
}
