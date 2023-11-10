using cerberus.Models.edmx;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.DTO.Reports
{
    public class FSWorkShiftReportFormDTO : ReportDTO
    {
        public int factorysite_id { get; set; }
        public int target_warehouse_id { get; set; }

        public Dictionary<string, string> produced { get; set; }
        public Dictionary<string, string> losses { get; set; }
        public Dictionary<string, string> remains { get; set; }

    }
    
}