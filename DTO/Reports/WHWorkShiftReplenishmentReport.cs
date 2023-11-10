using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.DTO.Reports
{
    public class WHWorkShiftReplenishmentReportFormDTO : ReportDTO
    {
        public int workshift_id { get; set; }
        public int warehouse_id { get; set; }

        public Dictionary<string, string> items { get; set; }



    }
}