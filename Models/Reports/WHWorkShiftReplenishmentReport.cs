using cerberus.DTO.Reports;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHWorkShiftReplenishmentReport : Report
    {
        public int workshift_id { get; set; }
        public int warehouse_id { get; set; }

        public Dictionary<int, int> items { get; set; }

        public WHWorkShiftReplenishmentReport() : base(Types.WHWorkShiftReplenishment) { items = new Dictionary<int, int>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static WHWorkShiftReplenishmentReport from(WHWorkShiftReplenishmentReportFormDTO dto)
        {
            var res = new WHWorkShiftReplenishmentReport();
            res.creator_id = dto.creator_id;
            res.department_id = dto.department_id;
            res.timestamp = dto.timestamp;

            res.warehouse_id = dto.warehouse_id;
            res.items = dto.items.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));
            return res;

        }

    }
}