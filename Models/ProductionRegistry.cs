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
            return db.ProductionRegistries.Where(r => produced_keys.Contains(r.production_id)).ToDictionary(kv => kv.requirement_id, kv => kv.count * produced[kv.production_id]).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        { acc[p.Key] = acc.Keys.Contains(p.Key) ? acc[p.Key] + p.Value : p.Value; return acc; }
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}