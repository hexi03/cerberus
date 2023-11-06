using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using static cerberus.Models.edmx.GroupDepartmentClaim;

    public partial class GroupFactorySiteClaim
    {
        internal static IQueryable<FactorySite> get_group_factorysites(CerberusDBEntities context, IList<string> group_ids)
        {

                if (group_ids.Any(p => p == "Admin"))
                {
                    return context.FactorySites.Include(e => e.Department);
                }
                var deps = GroupDepartmentClaim.get_group_departments(context, group_ids, Levels.Full).Select(e => e.id);
                var factorysites = context.GroupFactorySiteClaims
                        .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.FactorySite.department_id)).Select(c => c.FactorySite).Include(e => e.Department);
                return factorysites;
            
        }
    }
}
