﻿using cerberus.DTO.Reports;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHShipmentReport : WareHouseReport
    {
        public Dictionary<int, int> items { get; set; }

        public WHShipmentReport() : base(Types.WHShipment) { items = new Dictionary<int, int>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static WHShipmentReport from(WHShipmentReportFormDTO dto)
        {
            var res = new WHShipmentReport();
            res.creator_id = dto.creator_id;
            res.department_id = dto.department_id;
            res.timestamp = dto.timestamp;

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