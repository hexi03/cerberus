using System;

namespace cerberus.Models.ViewModels.Reports
{
    public class ReportViewModel
    {
        public int id { get; set; }
        public string report_type { get; set; }
        public string creator_id { get; set; }
        public int department_id { get; set; }
        public DateTime timestamp { get; set; }



        public string get_type_as_string()
        {
            switch (report_type)
            {
                case "FSSupplyRequirement":
                    return "Отчет о запросе снабжения";
                case "FSWorkShift":
                    return "Отчет о рабочей смене";
                case "WHWorkShiftReplenishment":
                    return "Отчет о приемке ПП смены";
                case "WHReplenishment":
                    return "Отчет о приемке";
                case "WHShipment":
                    return "Отчет об отгрузках";
                case "WHRelease":
                    return "Отчет о предоставлении РМ";
                case "WHInventarisation":
                    return "Отчет об инвентаризации";
                default:
                    return "Неизветный отчет";
            }
        }
    }
}