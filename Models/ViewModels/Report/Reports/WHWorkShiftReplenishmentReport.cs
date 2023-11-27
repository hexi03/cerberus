using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.Reports;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels.Reports
{
    public class WHWorkShiftReplenishmentReportFormViewModel
    {
        [ReportID]
        public int? id { get; set; } = null;

        [Required(ErrorMessage = "Идентификатор рабочей смены обязателен")]
        [ReportID]
        public int workshift_id { get; set; }

        [Required(ErrorMessage = "Идентификатор склада обязателен")]
        [WareHouseID]
        public int warehouse_id { get; set; }

        [ItemIDStringList(ErrorMessage = "Список предметов содержит некорректные данные")]
        public Dictionary<string, string> items { get; set; } = new Dictionary<string, string>();

        public class Mapper : ITypeConverter<WHWorkShiftReplenishmentReport, WHWorkShiftReplenishmentReportFormViewModel>
        {

            public WHWorkShiftReplenishmentReportFormViewModel Convert(WHWorkShiftReplenishmentReport source, WHWorkShiftReplenishmentReportFormViewModel destination, ResolutionContext context)
            {
                destination = new WHWorkShiftReplenishmentReportFormViewModel();
                destination.id = source.id;
                destination.workshift_id = source.workshift_id;
                destination.warehouse_id = source.warehouse_id;
                destination.items = source.items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return destination;
            }
        }
    }
}