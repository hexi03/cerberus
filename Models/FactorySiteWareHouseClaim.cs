﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.Models.edmx
{
    using cerberus.DTO;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Threading.Tasks;

    public partial class FactorySiteWareHouseClaim
    {
        public static async Task<List<WareHouseDTO>> get_warehouses(int factorysite_id)
        {
            var dbc = new CerberusDBEntities();
            var query = (from claim in dbc.FactorySiteWareHouseClaims
                         where claim.factorysite_id == factorysite_id
                         join wh in dbc.WareHouses
                         on claim.warehouse_id equals wh.id
                         select new WareHouseDTO
                         {
                             id = wh.id,
                             department_id = wh.department_id,
                             name = wh.name,

                         });

            return await query.ToListAsync<WareHouseDTO>();
        }

        public static async Task<List<FactorySite>> get_factorysites(int warehouse_id)
        {
            var dbc = new CerberusDBEntities();
            var query = (from claim in dbc.FactorySiteWareHouseClaims
                         where claim.warehouse_id == warehouse_id
                         join fs in dbc.FactorySites
                         on claim.factorysite_id equals fs.id
                         select new FactorySite
                         {
                             id = fs.id,
                             department_id = fs.department_id,
                             name = fs.name,

                         });


            return await query.ToListAsync();
        }
    }
}
