using cerberus.Models.edmx;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace cerberus.Models.Reports
{
    public class FactorySiteReport : Report
    {
        public int factorysite_id { get; set; }

        public FactorySiteReport(Types type) : base(type) { }

        public static async Task<IList<FactorySiteReport>> get_reports(CerberusDBEntities db, int factorysite_id)
        {
            return
                (await time_filter(
                    db.Reports
                    .Where(r =>
                        r.department_id == db.FactorySites.Where(fs => fs.id == factorysite_id).FirstOrDefault().department_id && (
                            r.report_type == Types.FSWorkShift.ToString() ||
                            r.report_type == Types.FSSupplyRequirement.ToString()
                        )
                    )
                ).ToListAsync()).Select(r => (FactorySiteReport)r.from_generic()).Where(r => r.factorysite_id == factorysite_id).ToList();
        }

        public string get_type_as_string()
        {
            switch ((Types)Enum.Parse(typeof(Types), report_type))
            {
                case Types.FSSupplyRequirement:
                    return "Отчет о запросе снабжения";
                case Types.FSWorkShift:
                    return "Отчет о рабочей смене";

                default:
                    return "Неизветный отчет";
            }
        }

    }
}
