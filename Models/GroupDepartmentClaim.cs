using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    public partial class GroupDepartmentClaim
    {
        public enum Levels{
            Full,
            Partial
        }
        internal static IQueryable<Department> get_group_departments(CerberusDBEntities context, IList<string> group_ids, Levels level)
        {
            

                if (group_ids.Any(p => p == "Admin"))
                {
                    return context.Departments;
                }
                var deps = context.GroupFactorySiteClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.FactorySite.Department)
                        .Union(context.GroupWareHouseClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Warehouse.Department));
                switch (level) {
                    

                    case Levels.Full:
                        return context.GroupDepartmentClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department);
                    case Levels.Partial:
                        return context.GroupDepartmentClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department).Union(deps);
                default:
                        throw new Exception();
                }

            
        }
    }
}