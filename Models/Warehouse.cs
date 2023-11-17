using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace cerberus.Models.edmx
{
    using cerberus.Models.Reports;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    [MetadataType(typeof(WareHouseMetadata))]
    public partial class Warehouse
    {
        public static Dictionary<int, int> get_storage_state(CerberusDBEntities db, int warehouse_id ) {
            var warehouse = db.WareHouses.Find( warehouse_id );

            var raw_r = Report.time_filter(db.Reports
                .Where(r => r.department_id == warehouse.department_id && r.report_type == Report.Types.WHInventarisation.ToString())).OrderByDescending(r => r.timestamp).FirstOrDefault();
            Dictionary<int, int> last_inv_item_list;
            DateTime timestamp;
            if (raw_r != null) {
                var inv_report = ((WHInventarisationReport)raw_r.from_generic());
                last_inv_item_list = inv_report.items;
                timestamp = inv_report.timestamp;
            }
            else {
                last_inv_item_list = new Dictionary<int, int>();
                timestamp = DateTime.MinValue;
            }

            var shipments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHShipment.ToString() && p.timestamp > timestamp && p.department_id == warehouse.department_id)).ToList()
                    .Select(p => (WHShipmentReport)p.from_generic()).Where(p => (p.warehouse_id == warehouse.id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );
            

            var replenishments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHReplenishment.ToString() && p.timestamp > timestamp && p.department_id == warehouse.department_id)).ToList()
                    .Select(p => (WHReplenishmentReport)p.from_generic()).Where(p => (p.warehouse_id == warehouse.id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            var wh_replenishments = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.timestamp > timestamp && p.department_id == warehouse.department_id)).ToList()
                    .Select(p => (WHWorkShiftReplenishmentReport)p.from_generic()).Where(p => (p.warehouse_id == warehouse.id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            var release = Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp > timestamp && p.department_id == warehouse.department_id)).ToList()
                    .Select(p => (WHReleaseReport)p.from_generic()).Where(p => (p.warehouse_id == warehouse.id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    );

            return
                misc.MergeDictionariesWithSum(
                    misc.MergeDictionariesWithSum(
                        misc.MergeDictionariesWithSum(shipments, release).ToDictionary(kv => kv.Key, kv => -kv.Value),
                        misc.MergeDictionariesWithSum(wh_replenishments, replenishments)
                        ),
                    last_inv_item_list
                ).Where(pair => pair.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);
        }


        public static (List<IError>, List<IWarning>) get_state(CerberusDBEntities db, int warehouse_id)
        {
            var bag = (Errors: new List<IError>(), Warnings: new List<IWarning>());
            bag.Errors = bag.Errors.Union(WHInventarisationReport.get_errors(db, warehouse_id)).ToList();
            bag.Warnings = bag.Warnings.Union(WHWorkShiftReplenishmentReport.get_warnings(db, warehouse_id)).ToList();
            bag.Warnings = bag.Warnings.Union(WHReleaseReport.get_warnings(db, warehouse_id)).ToList();
            if (has_invalid_storage_state(db, warehouse_id)) {
                bag.Errors.Add(new InvalidStorageStateError());
            }
            return bag;
        }

        public static bool has_invalid_storage_state(CerberusDBEntities db, int warehouse_id) {
            return get_storage_state(db, warehouse_id).Where(it => it.Value < 0).Count() > 0;
        }

        public partial class WareHouseMetadata
        {

            public int id { get; set; }

            [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
            [Display(Name = "Наименование")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
            //[RegularExpression("^[A-Za-z]+$", ErrorMessage = "Имя содержит недопустимые символы")]
            public string name { get; set; }

            [Required(ErrorMessage = "Отдел не выбран")]
            [Range(1, int.MaxValue, ErrorMessage = "Please select a valid department_id")]

            public int department_id { get; set; }

        }

        class InvalidStorageStateError : IError
        {
            string text_message;
            string html_message;
            public InvalidStorageStateError()
            {
                text_message = "Получение состояния из отчетности приводит к невозможному результату (рекомендуется перепроверить отчеты).";
                html_message =
                    "<p>Получение состояния из отчетности приводит к невозможному результату (рекомендуется перепроверить отчеты).</p>";

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
