using cerberus.Models.edmx;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.DTO.Reports
{
    public class FSSupplyRequirementReportFormDTO : ReportDTO
    {

        
        public int factorysite_id {get; set;}
        public int target_warehouse_id { get; set; }
        public Dictionary<string, string> items { get; set; }

        
    }
    
}