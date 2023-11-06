using cerberus.Models.edmx;
using Microsoft.Owin.Security.Twitter.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.Models.Reports
{
    public class FSSupplyRequirementReport : Report
    {
        public int factorysite_id {get; set;}
        public int target_warehouse_id { get; set; }
        //public int workplan_id;

        public Dictionary<string, string> items { get; set; }


        public FSSupplyRequirementReport() : base(Types.FSSupplyRequirement) {
            items = new Dictionary<string, string>();
        }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static async Task<IList<FSSupplyRequirementReport>> get_unsatisfied(CerberusDBEntities db)
        {
            return db.Reports
                .Where(r => r.report_type == Report.Types.FSSupplyRequirement.ToString()).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();
        }
        public static IList<FSSupplyRequirementReport> get_unsatisfied(CerberusDBEntities db, int department_id)
        {

            return db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString()).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IDictionary<int, int> get_unsatisfied_item_list(CerberusDBEntities db, int report_id)
        {

            var r = (FSSupplyRequirementReport)db.Reports
                .Find(report_id).from_generic();

            return misc.MergeDictionariesWithSum(
                db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp > r.timestamp).ToList()
                    .Select(p => (WHReleaseReport)r.from_generic()).Where(p => (p.supply_requirement_id == r.id) && (p.warehouse_id == r.target_warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value)))
                    ),
                r.items.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value))
            );


        }

    }
    
}