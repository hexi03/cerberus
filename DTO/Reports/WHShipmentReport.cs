using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Web;

namespace cerberus.DTO.Reports
{
    [MetadataType(typeof(WHShipmentReportFormDTOMetadata))]
    public class WHShipmentReportFormDTO : ReportDTO
    {
        public int warehouse_id { get; set; }
        public Dictionary<string, string> items { get; set; }

        private class WHShipmentReportFormDTOMetadata
        {
            [Required(ErrorMessage = "Идентификатор склада обязателен")]
            public int warehouse_id { get; set; }

            [ItemList(ErrorMessage = "Список предметов содержит некорректные данные")]
            public Dictionary<string, string> items { get; set; }
        }

        public class Mapper
        {
            public static WHShipmentReportFormDTO map(WHShipmentReport rep)
            {
                var res = new WHShipmentReportFormDTO();

                res.timestamp = rep.timestamp;
                res.creator_id = rep.creator_id;
                res.department_id = rep.department_id;
                res.report_type = rep.report_type;

                res.warehouse_id = rep.warehouse_id;
                res.items = rep.items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return res;
            }
        }
    }
}