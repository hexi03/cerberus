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

    public partial class GroupWareHouseClaim
    {

        internal static IQueryable<Warehouse> get_group_warehouses(CerberusDBEntities context, IList<string> group_ids)
        {

                if (group_ids.Any(p => p == "Admin"))
                {
                    return context.WareHouses.Include(e => e.Department);
                }

                var deps = GroupDepartmentClaim.get_group_departments(context, group_ids, Levels.Full).Select(e => e.id);

                var warehouses = context.GroupWareHouseClaims
                    .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.Warehouse.department_id)).Select(c => c.Warehouse).Include(e => e.Department);

                return warehouses;
            
        }
    }
}