namespace cerberus.Models.edmx
{
    using cerberus.Models.Reports;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public partial class Warehouse
    {
        public static Dictionary<int, int> get_state(CerberusDBEntities db, int warehouse_id ) {
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
                        misc.MergeDictionariesWithSum(shipments, release),
                        misc.MergeDictionariesWithSum(wh_replenishments, replenishments).ToDictionary(kv => kv.Key, kv => -kv.Value)
                        ),
                    last_inv_item_list
                );
        }
    }
}