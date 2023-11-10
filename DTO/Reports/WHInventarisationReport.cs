using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.DTO.Reports
{
    public class WHInventarisationReportFormDTO : ReportDTO
    {
        public int warehouse_id { get; set; }

        public Dictionary<string, string> items { get; set; }

    }
}