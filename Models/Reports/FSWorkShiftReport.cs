using cerberus.Models.edmx;
using Microsoft.Ajax.Utilities;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.Models.Reports
{
    public class FSWorkShiftReport : Report
    {
        public int factorysite_id { get; set; }
        public int target_warehouse_id { get; set; }
        //public int workplan_id;

        public Dictionary<string, string> produced { get; set; }
        public Dictionary<string, string> losses { get; set; }
        public Dictionary<string, string> remains { get; set; }

        public FSWorkShiftReport() : base(Types.FSWorkShift) {
            produced = new Dictionary<string, string>();
            losses = new Dictionary<string, string>();
            remains = new Dictionary<string, string>();
        }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }
        public static async Task<IList<FSWorkShiftReport>> get_unsatisfied(CerberusDBEntities db) {
            return db.Reports
                .Where(r => r.report_type == Report.Types.FSWorkShift.ToString()).ToList()
                .Select(r => (FSWorkShiftReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();
        }
        public static IList<FSWorkShiftReport> get_unsatisfied(CerberusDBEntities db, int department_id)
        {

            return db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.FSWorkShift.ToString()).ToList()
                .Select(r => (FSWorkShiftReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db,r.id).Count() != 0
                ).ToList();

        }

        public static IDictionary<int,int> get_unsatisfied_item_list(CerberusDBEntities db, int report_id)
        {
            var r_raw = db.Reports
                .Find(report_id);
            var r = (FSWorkShiftReport)(r_raw.from_generic());

            return misc.MergeDictionariesWithSum(
                db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.timestamp > r.timestamp).ToList()
                    .Select(p => (WHWorkShiftReplenishmentReport)r.from_generic()).Where(p => (p.workshift_id == r.id) && (p.warehouse_id == r.target_warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value)))
                    ),
                r.produced.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value))
            );
                

        }


    }
    
}