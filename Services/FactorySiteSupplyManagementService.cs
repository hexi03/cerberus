using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace cerberus.Services
{
    public interface IFactorySiteSupplyManagementService
    {
        Task updateSupplySources(FactorySiteSupplyEditModel vm);
        Task<List<Warehouse>> get_warehouses(int factorysite_id);
        Task<List<FactorySite>> get_factorysites(int warehouse_id);

    }
    public class FactorySiteSupplyManagementService : IFactorySiteSupplyManagementService
    {
        CerberusDBEntities _db;
        public FactorySiteSupplyManagementService(CerberusDBEntities db)
        {
            _db = db;
        }
        public async Task updateSupplySources(FactorySiteSupplyEditModel vm)
        {
            _db.FactorySiteWareHouseClaims.RemoveRange(_db.FactorySiteWareHouseClaims.Where(e => e.factorysite_id == vm.id));

            foreach (var wh_id in vm.getWareHouseIDs())
            {
                _db.FactorySiteWareHouseClaims.Add(new FactorySiteWareHouseClaim()
                {
                    warehouse_id = wh_id,
                    factorysite_id = vm.id
                });
            }
            await _db.SaveChangesAsync();
        }

        public async Task<List<Warehouse>> get_warehouses(int factorysite_id)
        {

            var query = _db.FactorySiteWareHouseClaims.
                         Where(claim => claim.factorysite_id == factorysite_id)
                         .Select(c => c.Warehouse);

            return await query.ToListAsync<Warehouse>();
        }

        public async Task<List<FactorySite>> get_factorysites(int warehouse_id)
        {

            var query = _db.FactorySiteWareHouseClaims.
                         Where(claim => claim.warehouse_id == warehouse_id)
                         .Select(c => c.FactorySite);


            return await query.ToListAsync();
        }
    }
}