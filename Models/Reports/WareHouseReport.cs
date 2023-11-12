using cerberus.DTO.Reports;
using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace cerberus.Models.Reports
{
    public class WareHouseReport : Report
    {
        public int warehouse_id { get; set; }

        public WareHouseReport(Types type) : base(type) { }

        public static async Task<IList<WareHouseReport>> get_reports(CerberusDBEntities db, int warehouse_id) {
            return
                (await time_filter(
                    db.Reports
                    .Where(r =>
                        r.department_id == db.WareHouses.Where(wh => wh.id == warehouse_id).FirstOrDefault().department_id && (
                            r.report_type == Types.WHInventarisation.ToString() ||
                            r.report_type == Types.WHShipment.ToString() ||
                            r.report_type == Types.WHRelease.ToString() ||
                            r.report_type == Types.WHReplenishment.ToString() ||
                            r.report_type == Types.WHWorkShiftReplenishment.ToString()
                        )
                    )
                ).ToListAsync())
                .Select(r => (WareHouseReport)r.from_generic()).Where(r => r.warehouse_id == warehouse_id).ToList();
        }

        public string get_type_as_string() {
            switch ((Types)Enum.Parse(typeof(Types), report_type))
            {
                case Types.WHWorkShiftReplenishment:
                    return "Отчет о приемке ПП смены";
                case Types.WHReplenishment:
                    return "Отчет о приемке";
                case Types.WHShipment:
                    return "Отчет об отгрузках";
                case Types.WHRelease:
                    return "Отчет о предоставлении РМ";
                case Types.WHInventarisation:
                    return "Отчет об инвентаризации";
                default:
                    return "Неизветный отчет";
            }
        }
    }
}