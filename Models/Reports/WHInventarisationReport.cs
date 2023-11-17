using cerberus.DTO.Reports;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.Models.Reports
{
    public class WHInventarisationReport : WareHouseReport
    {

        public Dictionary<int, int> items { get; set; }

        public WHInventarisationReport() : base(Types.WHInventarisation) { items = new Dictionary<int, int>(); }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static WHInventarisationReport from(WHInventarisationReportFormDTO dto)
        {
            var res = new WHInventarisationReport();
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


        public static async Task<IList<WHInventarisationReport>> get_unsatisfied(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports
                .Where(r => r.report_type == Report.Types.WHInventarisation.ToString())).ToList()
                .Select(r => (WHInventarisationReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();
        }
        public static IList<WHInventarisationReport> get_unsatisfied(CerberusDBEntities db, int department_id)
        {

            return Report.time_filter(db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.WHInventarisation.ToString())).ToList()
                .Select(r => (WHInventarisationReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IList<WHInventarisationReport> get_unsatisfied_wh(CerberusDBEntities db, int warehouse_id)
        {
            Warehouse wh = db.WareHouses.Find(warehouse_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == wh.department_id && r.report_type == Report.Types.WHInventarisation.ToString())).ToList()
                .Select(r => (WHInventarisationReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.warehouse_id == warehouse_id &&
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }




        public static IDictionary<int, int> get_unsatisfied_item_list(CerberusDBEntities db, int report_id)
        {
            var r_raw = db.Reports
                .Find(report_id);
            var r = (WHInventarisationReport)(r_raw.from_generic());

            var shipments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHShipment.ToString() && p.timestamp < r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHShipmentReport)p.from_generic()).Where(p => (p.warehouse_id == r.warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );
            var replenishments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHReplenishment.ToString() && p.timestamp < r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHReplenishmentReport)p.from_generic()).Where(p => (p.warehouse_id == r.warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            var wh_replenishments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.timestamp < r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHWorkShiftReplenishmentReport)p.from_generic()).Where(p => (p.warehouse_id == r.warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            var release = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp < r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHReleaseReport)p.from_generic()).Where(p => (p.warehouse_id == r.warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            return misc.MergeDictionariesWithSum(
                misc.MergeDictionariesWithSum(
                    misc.MergeDictionariesWithSum(shipments,release),
                    misc.MergeDictionariesWithSum(wh_replenishments, replenishments).ToDictionary(kv => kv.Key, kv => -kv.Value)
                    ),
                r.items
            );

        }


        public static IList<IError> get_errors(CerberusDBEntities db, int warehouse_id)
        {
            return (IList<IError>)get_unsatisfied_wh(db,warehouse_id).Select(r => (IError)new UnsatisfiedError(r)).ToList();

        }




        class UnsatisfiedError : IError
        {
            string text_message;
            string html_message;
            public UnsatisfiedError(WHInventarisationReport rep) {
                text_message = "Состояние склада не соответствует отчету об инвентаризации " + rep.id.ToString();
                html_message =
                    "<p>Состояние склада не соответствует <a href='/Reports/Details/" + rep.id + "'>отчету об инвентаризации</a> </p>";
                    
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