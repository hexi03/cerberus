using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.Reports;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.DTO.Reports
{
    [MetadataType(typeof(FSWorkShiftReportFormDTOValidation))]
    public class FSWorkShiftReportFormDTO : ReportDTO
    {
        public int factorysite_id { get; set; }
        public int target_warehouse_id { get; set; }
        public Dictionary<string, string> produced { get; set; }
        public Dictionary<string, string> losses { get; set; }
        public Dictionary<string, string> remains { get; set; }

        private class FSWorkShiftReportFormDTOValidation
        {
            [Required(ErrorMessage = "Идентификатор завода обязателен")]
            public int factorysite_id { get; set; }

            [Required(ErrorMessage = "Идентификатор целевого склада обязателен")]
            public int target_warehouse_id { get; set; }

            [ItemList(ErrorMessage = "Список произведенных продуктов содержит некорректные данные")]
            public Dictionary<string, string> produced { get; set; }

            [ItemList(ErrorMessage = "Список потерь содержит некорректные данные")]
            public Dictionary<string, string> losses { get; set; }

            [ItemList(ErrorMessage = "Список остатков содержит некорректные данные")]
            public Dictionary<string, string> remains { get; set; }
        }

        public class Mapper
        {
            public static FSWorkShiftReportFormDTO map(FSWorkShiftReport rep)
            {
                var res = new FSWorkShiftReportFormDTO();

                res.timestamp = rep.timestamp;
                res.creator_id = rep.creator_id;
                res.department_id = rep.department_id;
                res.report_type = rep.report_type;

                res.factorysite_id = rep.factorysite_id; ;
                res.target_warehouse_id = rep.target_warehouse_id;
                res.produced = rep.produced.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                res.losses = rep.losses.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                res.remains = rep.remains.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return res;
            }
        }
    }


}