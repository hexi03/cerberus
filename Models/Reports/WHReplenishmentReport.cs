using cerberus.Models.edmx;
using cerberus.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace cerberus.Models.Reports
{
    public class WHReplenishmentReport : WareHouseReport
    {

        public Dictionary<int, int> items { get; set; }

        public WHReplenishmentReport() : base(Types.WHReplenishment) { items = new Dictionary<int, int>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static WHReplenishmentReport from(WHReplenishmentReportFormViewModel dto)
        {
            var res = new WHReplenishmentReport();

            res.warehouse_id = dto.warehouse_id;
            if (dto.items != null)
            {
                res.items = dto.items.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));
            }
            else
            {
                res.items = new Dictionary<int, int>();
            }
            return res;

        }


    }
}