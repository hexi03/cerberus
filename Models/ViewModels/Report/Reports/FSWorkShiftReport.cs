using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.Reports;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels.Reports
{
    public class FSWorkShiftReportFormViewModel
    {
        [ReportID]
        public int? id { get; set; } = null;

        [Required(ErrorMessage = "Идентификатор завода обязателен")]
        [FactorySiteID]
        public int factorysite_id { get; set; }

        [Required(ErrorMessage = "Идентификатор целевого склада обязателен")]
        [WareHouseID]
        public int target_warehouse_id { get; set; }

        [ItemIDStringList(ErrorMessage = "Список произведенных продуктов содержит некорректные данные")]
        public Dictionary<string, string> produced { get; set; } = new Dictionary<string, string>();

        [ItemIDStringList(ErrorMessage = "Список потерь содержит некорректные данные")]
        public Dictionary<string, string> losses { get; set; } = new Dictionary<string, string>();

        [ItemIDStringList(ErrorMessage = "Список остатков содержит некорректные данные")]
        public Dictionary<string, string> remains { get; set; } = new Dictionary<string, string>();


        public class Mapper : ITypeConverter<FSWorkShiftReport, FSWorkShiftReportFormViewModel>
        {

            public FSWorkShiftReportFormViewModel Convert(FSWorkShiftReport source, FSWorkShiftReportFormViewModel destination, ResolutionContext context)
            {
                destination = new FSWorkShiftReportFormViewModel();
                destination.id = source.id;
                destination.factorysite_id = source.factorysite_id;
                destination.target_warehouse_id = source.target_warehouse_id;
                destination.produced = source.produced.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                destination.losses = source.losses.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                destination.remains = source.remains.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return destination;
            }
        }




    }


}