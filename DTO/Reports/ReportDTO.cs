using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cerberus.DTO.Reports
{
    public class ReportDTO
    {
        public string report_type { get; set; }
        public string creator_id { get; set; }
        public int department_id { get; set; }
        public DateTime timestamp { get; set; }
    }
}