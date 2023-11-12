using cerberus.Models.edmx;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.ComponentModel.DataAnnotations;
using cerberus.Models;
using cerberus.Models.Reports;

namespace cerberus.DTO.Reports
{
    [MetadataType(typeof(FSSupplyRequirementReportFormDTOMetadata))]
    public class FSSupplyRequirementReportFormDTO : ReportDTO
    {

        
        public int factorysite_id {get; set;}
        public int target_warehouse_id { get; set; }
        public Dictionary<string, string> items { get; set; }

        public class FSSupplyRequirementReportFormDTOMetadata
        {
            [Required(ErrorMessage = "Идентификатор завода обязателен")]
            public int factorysite_id { get; set; }

            [Required(ErrorMessage = "Идентификатор целевого склада обязателен")]
            public int target_warehouse_id { get; set; }

            [ItemList(ErrorMessage = "Список предметов содержит некорректные данные")]
            public Dictionary<string, string> items { get; set; }
        }

        public class Mapper
        {
            public static FSSupplyRequirementReportFormDTO map(FSSupplyRequirementReport rep)
            {
                var res = new FSSupplyRequirementReportFormDTO();

                res.timestamp = rep.timestamp;
                res.creator_id = rep.creator_id;
                res.department_id = rep.department_id;
                res.report_type = rep.report_type;

                res.factorysite_id = rep.factorysite_id; ;
                res.target_warehouse_id = rep.target_warehouse_id;
                res.items = rep.items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return res;
            }
        }
    }


    
}