using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.Reports;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels.Reports
{
    public class WHInventarisationReportFormViewModel
    {

        [ReportID]
        public int? id { get; set; } = null;

        [Required(ErrorMessage = "Идентификатор склада обязателен")]
        [WareHouseID]
        public int warehouse_id { get; set; }

        [ItemIDStringList(ErrorMessage = "Список предметов содержит некорректные данные")]
        public Dictionary<string, string> items { get; set; } = new Dictionary<string, string>();


        public class Mapper : ITypeConverter<WHInventarisationReport, WHInventarisationReportFormViewModel>
        {

            public WHInventarisationReportFormViewModel Convert(WHInventarisationReport source, WHInventarisationReportFormViewModel destination, ResolutionContext context)
            {
                destination = new WHInventarisationReportFormViewModel();
                destination.id = source.id;
                destination.warehouse_id = source.warehouse_id;
                destination.items = source.items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return destination;
            }
        }
    }
}