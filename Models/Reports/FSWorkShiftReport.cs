﻿using cerberus.Models.edmx;
using cerberus.Models.ViewModels.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace cerberus.Models.Reports
{
    public class FSWorkShiftReport : FactorySiteReport
    {
        public int target_warehouse_id { get; set; }

        public Dictionary<int, int> produced { get; set; }
        public Dictionary<int, int> losses { get; set; }
        public Dictionary<int, int> remains { get; set; }

        public FSWorkShiftReport() : base(Types.FSWorkShift)
        {
            produced = new Dictionary<int, int>();
            losses = new Dictionary<int, int>();
            remains = new Dictionary<int, int>();
        }

        public static FSWorkShiftReport from(FSWorkShiftReportFormViewModel dto)
        {
            var res = new FSWorkShiftReport();

            res.factorysite_id = dto.factorysite_id;
            res.target_warehouse_id = dto.target_warehouse_id;
            if (dto.produced != null)
            {
                res.produced = dto.produced.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));
            }
            else
            {
                res.produced = new Dictionary<int, int>();
            }

            if (dto.losses != null)
            {
                res.losses = dto.losses.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));
            }
            else
            {
                res.losses = new Dictionary<int, int>();
            }

            if (dto.remains != null)
            {
                res.remains = dto.remains.ToDictionary(kv => Convert.ToInt32(kv.Key), kv => Convert.ToInt32(kv.Value));
            }
            else
            {
                res.remains = new Dictionary<int, int>();
            }


            return res;

        }

        public Report to_generic()
        {
            serialized = JsonSerializer.Serialize(this);
            return new Report(this);
        }
        public static async Task<IList<FSWorkShiftReport>> get_unsatisfied(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports
                .Where(r => r.report_type == Report.Types.FSWorkShift.ToString())).ToList()
                .Select(r => (FSWorkShiftReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();
        }
        public static IList<FSWorkShiftReport> get_unsatisfied(CerberusDBEntities db, int department_id)
        {

            return Report.time_filter(db.Reports
                .Where(r => r.department_id == department_id && r.report_type == Report.Types.FSWorkShift.ToString())).ToList()
                .Select(r => (FSWorkShiftReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IList<FSWorkShiftReport> get_unsatisfied_target_wh(CerberusDBEntities db, int target_warehouse_id)
        {
            Warehouse wh = db.WareHouses.Find(target_warehouse_id);
            return Report.time_filter(db.Reports
                .Where(r => r.department_id == wh.department_id && r.report_type == Report.Types.FSWorkShift.ToString())).ToList()
                .Select(r => (FSWorkShiftReport)r.from_generic()).ToList()
                .Where(
                    r =>
                    r.target_warehouse_id == target_warehouse_id &&
                    get_unsatisfied_item_list(db, r.id).Count() != 0
                ).ToList();

        }

        public static IDictionary<int, int> get_unsatisfied_item_list(CerberusDBEntities db, int report_id)
        {
            var r_raw = db.Reports
                .Find(report_id);
            var r = (FSWorkShiftReport)(r_raw.from_generic());

            var aaa0 = db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.department_id == r.department_id).ToList();

            var aaa = db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.timestamp > r.timestamp && p.department_id == r.department_id).ToList()
                    .Select(p => (WHWorkShiftReplenishmentReport)p.from_generic()).Where(p => (p.workshift_id == r.id) && (p.warehouse_id == r.target_warehouse_id)).ToList();
            return misc.MergeDictionariesWithSum(
                db.Reports
                    .Where(p => p.report_type == Report.Types.WHWorkShiftReplenishment.ToString() && p.timestamp > r.timestamp && p.department_id == r.department_id).ToList()
                    .Select(p => (WHWorkShiftReplenishmentReport)p.from_generic()).Where(p => (p.workshift_id == r.id) && (p.warehouse_id == r.target_warehouse_id))
                    .Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.items)
                    ).ToDictionary(kv => kv.Key, kv => -kv.Value),
                r.produced
            ).Where(pair => pair.Value != 0).ToDictionary(kv => kv.Key, kv => kv.Value);

        }

        public static IList<FSWorkShiftReport> get_reports_with_losses(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString())).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.losses.Count() > 0).ToList();
        }

        public static IList<FSWorkShiftReport> get_reports_with_remains(CerberusDBEntities db)
        {
            return Report.time_filter(db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString())).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.remains.Count() > 0).ToList();
        }

        public static IList<FSWorkShiftReport> get_reports_with_losses(CerberusDBEntities db, int factorysite_id)
        {
            FactorySite fs = db.FactorySites.Find(factorysite_id);
            return Report.time_filter(db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString() && p.department_id == fs.department_id)).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.losses.Count() > 0 && p.factorysite_id == factorysite_id).ToList();
        }

        public static IList<FSWorkShiftReport> get_reports_with_remains(CerberusDBEntities db, int factorysite_id)
        {
            FactorySite fs = db.FactorySites.Find(factorysite_id);
            return Report.time_filter(db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString() && p.department_id == fs.department_id)).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.remains.Count() > 0 && p.factorysite_id == factorysite_id).ToList();
        }






        public static IList<IWarning> get_warnings(CerberusDBEntities db, int factorysite_id)
        {
            var res = new List<IWarning>();
            res = res.Union((IList<IWarning>)get_reports_with_losses(db, factorysite_id).Select(r => (IWarning)new LossesWarning(r)).ToList()).ToList();
            //res = res.Union((IList<IWarning>)get_reports_with_remains(db, factorysite_id).Select(r => (IWarning)new RemainsWarning(r)).ToList()).ToList();
            return res;
        }

        class LossesWarning : IWarning
        {
            string text_message;
            string html_message;
            public LossesWarning(FSWorkShiftReport rep)
            {
                text_message = "В результате рабочей смены " + rep.timestamp.ToString() + " произошла потеря расходных материалов";
                html_message =
                    "<p>В результате <a href='/Reports/Details/" + rep.id + "'>рабочей смены " + rep.timestamp.ToString() + "</a>  произошла потеря расходных материалов</p>";

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

        class RemainsWarning : IWarning
        {
            string text_message;
            string html_message;
            public RemainsWarning(FSWorkShiftReport rep)
            {
                text_message = "По окончании рабочей смены " + rep.timestamp.ToString() + " были задекларированы остатки РМ";
                html_message =
                    "<p>По окончании <a href='/Reports/Details/" + rep.id + "'>рабочей смены " + rep.timestamp.ToString() + "</a>  были задекларированы остатки РМ</p>";

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