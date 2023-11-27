using AutoMapper;
using cerberus.Attributes.Validation;
using cerberus.Models.Reports;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace cerberus.Models.ViewModels.Reports
{
    public class FSSupplyRequirementReportFormViewModel
    {

        [ReportID]
        public int? id { get; set; } = null;

        [Required(ErrorMessage = "Идентификатор завода обязателен")]
        [FactorySiteID]
        public int factorysite_id { get; set; }

        [Required(ErrorMessage = "Идентификатор целевого склада обязателен")]
        [WareHouseID]
        public int target_warehouse_id { get; set; }

        [ItemIDStringList(ErrorMessage = "Список предметов содержит некорректные данные")]
        public Dictionary<string, string> items { get; set; } = new Dictionary<string, string>();


        public class Mapper : ITypeConverter<FSSupplyRequirementReport, FSSupplyRequirementReportFormViewModel>
        {

            public FSSupplyRequirementReportFormViewModel Convert(FSSupplyRequirementReport source, FSSupplyRequirementReportFormViewModel destination, ResolutionContext context)
            {
                destination = new FSSupplyRequirementReportFormViewModel();
                destination.id = source.id;
                destination.factorysite_id = source.factorysite_id; ;
                destination.target_warehouse_id = source.target_warehouse_id;
                destination.items = source.items.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value.ToString());
                return destination;
            }
        }
    }



}