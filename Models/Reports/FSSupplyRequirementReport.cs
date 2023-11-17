using cerberus.DTO.Reports;
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
    public class FSSupplyRequirementReport : FactorySiteReport
    {

        public int target_warehouse_id { get; set; }
        public Dictionary<int, int> items { get; set; }


        public FSSupplyRequirementReport() : base(Types.FSSupplyRequirement) {
            items = new Dictionary<int, int>();
        }

        public static FSSupplyRequirementReport from(FSSupplyRequirementReportFormDTO dto) {
            var res = new FSSupplyRequirementReport();
            res.creator_id = dto.creator_id ;
            res.department_id = dto.department_id;
            res.timestamp = dto.timestamp;

            res.factorysite_id = dto.factorysite_id;
            res.target_warehouse_id = dto.target_warehouse_id;
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

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }

        public static async Task<IList<FSSupplyRequirementReport>> get_unsatisfied(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports
                .Where(r => r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();
        }
        public static IList<FSSupplyRequirementReport> get_unsatisfied(CerberusDBEntities db, int department_id)
        {

            return Report.time_filter(db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IList<FSSupplyRequirementReport> get_unsatisfied_fs(CerberusDBEntities db, int factorysite_id)
        {
            FactorySite fs = db.FactorySites.Find(factorysite_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == fs.department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.factorysite_id == factorysite_id &&
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IList<FSSupplyRequirementReport> get_unsatisfied_target_wh(CerberusDBEntities db, int target_warehouse_id)
        {
            Warehouse fs = db.WareHouses.Find(target_warehouse_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == fs.department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.target_warehouse_id == target_warehouse_id &&
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }








        public static async Task<IList<FSSupplyRequirementReport>> get_satisfied(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports
                .Where(r => r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() == 0
                ).ToList();
        }
        public static IList<FSSupplyRequirementReport> get_satisfied(CerberusDBEntities db, int department_id)
        {

            return Report.time_filter(db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() == 0
                ).ToList();

        }

        public static IList<FSSupplyRequirementReport> get_satisfied_fs(CerberusDBEntities db, int factorysite_id)
        {
            FactorySite fs = db.FactorySites.Find(factorysite_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == fs.department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.factorysite_id == factorysite_id &&
                    get_unsatisfied_item_list(db, r.id).Count() == 0
                ).ToList();

        }

        public static IList<FSSupplyRequirementReport> get_satisfied_target_wh(CerberusDBEntities db, int target_warehouse_id)
        {
            Warehouse fs = db.WareHouses.Find(target_warehouse_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == fs.department_id && r.report_type == Report.Types.FSSupplyRequirement.ToString())).ToList()
                .Select(r => (FSSupplyRequirementReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.target_warehouse_id == target_warehouse_id &&
                    get_unsatisfied_item_list(db, r.id).Count() == 0
                ).ToList();

        }









        public static IDictionary<int, int> get_unsatisfied_item_list(CerberusDBEntities db, int report_id)
        {

            var r = (FSSupplyRequirementReport)db.Reports
                .Find(report_id).from_generic();

            var aaa011 =
                db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.department_id == r.department_id).ToList();


            var aaa01 =
                db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp >= r.timestamp && p.department_id == r.department_id).ToList();


            var aaa0 =
                Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp >= r.timestamp && p.department_id == r.department_id)).ToList();

            var aaa = 
                Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp >= r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHReleaseReport)p.from_generic()).Where(p => (p.supply_requirement_id == r.id) && (p.warehouse_id == r.target_warehouse_id)).ToList();
            return misc.MergeDictionariesWithSum(
                Report.time_filter(db.Reports
                    .Where(p => p.report_type == Report.Types.WHRelease.ToString() && p.timestamp >= r.timestamp && p.department_id == r.department_id)).ToList()
                    .Select(p => (WHReleaseReport)p.from_generic()).Where(p => (p.supply_requirement_id == r.id) && (p.warehouse_id == r.target_warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    ).ToDictionary(kv => kv.Key, kv => -kv.Value),
                r.items
            ).Where(pair => pair.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);

        }

        public static IList<IWarning> get_errors(CerberusDBEntities db, int target_warehouse_id)
        {
            return (IList<IWarning>)get_unsatisfied_target_wh(db, target_warehouse_id).Select(r => new UnsatisfiedWarning(r)).ToList();

        }

        class UnsatisfiedWarning : IWarning
        {
            string text_message;
            string html_message;
            public UnsatisfiedWarning(FSSupplyRequirementReport rep)
            {
                text_message = "Есть запрошенные РМ " + rep.timestamp.ToString();
                html_message =
                    "<p>Есть запрошенные РМ для <a href='/Reports/Details/" + rep.id + "'> рабочей смены " + rep.timestamp.ToString() + "</a> </p>";

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