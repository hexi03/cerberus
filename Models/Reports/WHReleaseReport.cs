using cerberus.Models.edmx;
using cerberus.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace cerberus.Models.Reports
{
    public class WHReleaseReport : WareHouseReport
    {
        public int supply_requirement_id { get; set; }

        public Dictionary<int, int> items { get; set; }

        public WHReleaseReport() : base(Types.WHRelease) { items = new Dictionary<int, int>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static WHReleaseReport from(WHReleaseReportFormViewModel dto)
        {
            var res = new WHReleaseReport();

            res.warehouse_id = dto.warehouse_id;
            res.supply_requirement_id = dto.supply_requirement_id;
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

        public static IList<IWarning> get_warnings(CerberusDBEntities db, int warehouse_id)
        {
            return (IList<IWarning>)FSSupplyRequirementReport.get_unsatisfied_target_wh(db, warehouse_id).Select(r => (IWarning)new UnsatisfiedWarning(r)).ToList();

        }


        class UnsatisfiedWarning : IWarning
        {
            string text_message;
            string html_message;
            public UnsatisfiedWarning(FSSupplyRequirementReport rep)
            {
                text_message = "Не выданы запрошенные РМ " + rep.timestamp;
                html_message =
                    "<p>Не выданы запрошенные РМ для <a href='/Reports/Details/" + rep.id + "'>смены " + rep.timestamp + "</a></p>";
            }
            public string get_html()
            {
                return html_message;
            }

            public string get_message()
            {
                return text_message;
            }
        }


    }
}