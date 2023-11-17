using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.Models.edmx
{
    public partial class ProductionRegistry
    {
        public static async Task<Dictionary<int, int>> get_costs(CerberusDBEntities db, IDictionary<int, int> produced) {
            var produced_keys = produced.Keys.ToList();
            return db.ProductionRegistries.Where(r => produced_keys.Contains(r.production_id)).ToDictionary(kv => kv.requirement_id, kv => kv.count);
        }
    }
}