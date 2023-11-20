using cerberus.Models.Reports;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;

namespace cerberus.Models.edmx
{
    public partial class Report
    {

        public enum Types
        {
            WHInventarisation,
            WHReplenishment,
            WHRelease,
            WHShipment,
            WHWorkShiftReplenishment,
            FSWorkShift,
            FSSupplyRequirement,
//            MWorkPlan,
  //          MProductments,
    //        MSales,
      //      MInventarisationPlan,

        }
        public Report(Types type)
        {
            report_type = type.ToString();
            id = -1;
        }

        public Report()
        {
            id = -1;
        }

        public Report(Report r)
        {
            id = r.id;

            report_type = r.report_type;

            creator_id = r.creator_id;

            serialized = r.serialized;

            department_id = r.department_id;

            timestamp = r.timestamp;

        }

        public static void save(CerberusDBEntities db, Report rep)
        {

            if (rep.id == -1)
            {
                db.Reports.Add(rep);
            }
            else
            {
                var e = db.Reports.Find(rep.id);
                e.department_id = rep.department_id;
                e.timestamp = rep.timestamp;
                e.report_type   = rep.report_type;
                e.creator_id = rep.creator_id;
                e.serialized = rep.serialized;
                db.Reports.AddOrUpdate(e);
            }

        }

        
        public Report from_generic()
        {
            Report buf = null;
            switch ((Types)Enum.Parse(typeof(Types), report_type))
            {
                case Types.WHRelease:
                    buf = JsonSerializer.Deserialize<WHReleaseReport>(serialized);
                    break;
                case Types.WHInventarisation:
                    buf = JsonSerializer.Deserialize<WHInventarisationReport>(serialized);
                    break;
                case Types.WHReplenishment:
                    buf = JsonSerializer.Deserialize<WHReplenishmentReport>(serialized);
                    break;
                case Types.WHShipment:
                    buf = JsonSerializer.Deserialize<WHShipmentReport>(serialized);
                    break;
                case Types.WHWorkShiftReplenishment:
                    buf = JsonSerializer.Deserialize<WHWorkShiftReplenishmentReport>(serialized);
                    break;
                case Types.FSWorkShift:
                    buf = JsonSerializer.Deserialize<FSWorkShiftReport>(serialized);
                    break;
                case Types.FSSupplyRequirement:
                    buf = JsonSerializer.Deserialize<FSSupplyRequirementReport>(serialized);
                    break;
            }
            buf.id = id;
            buf.timestamp = timestamp;
            buf.creator_id = creator_id;
            buf.department_id = department_id;
            return buf;

        }

        public static IQueryable<Report> time_filter(IQueryable<Report> q)
        {
            var ts = DateTime.Today.AddMonths(Consts.REPORT_ACTUAL_TIME_DURATION);
            return q.Where(e => e.timestamp > ts);
        }

        public static IQueryable<Report> time_filter_last_day(IQueryable<Report> q)
        {
            var ts = DateTime.Today.AddDays(0);
            return q.Where(e => e.timestamp <= ts);
        }

        public static IList<Report> time_filter_last_day(IList<Report> q)
        {
            var ts = DateTime.Today.AddDays(0);
            return q.Where(e => e.timestamp <= ts).ToList();
        }


        public string get_type_as_string()
        {
            switch ((Types)Enum.Parse(typeof(Types), report_type))
            {
                case Types.FSSupplyRequirement:
                    return "Отчет о запросе снабжения";
                case Types.FSWorkShift:
                    return "Отчет о рабочей смене";
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