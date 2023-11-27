using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cerberus.Services
{
    public interface IProductionRegistryService
    {
        Task<Dictionary<int, int>> get_costs(IDictionary<int, int> produced);
    }
    public class ProductionRegistryService : IProductionRegistryService
    {
        CerberusDBEntities _db;

        public ProductionRegistryService(CerberusDBEntities db)
        {
            _db = db;
        }
        public async Task<Dictionary<int, int>> get_costs(IDictionary<int, int> produced)
        {
            var produced_keys = produced.Keys.ToList();
            return _db.ProductionRegistries.Where(r => produced_keys.Contains(r.production_id)).ToDictionary(kv => kv.requirement_id, kv => kv.count * produced[kv.production_id]).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        { acc[p.Key] = acc.Keys.Contains(p.Key) ? acc[p.Key] + p.Value : p.Value; return acc; }
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}