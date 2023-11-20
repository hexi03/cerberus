using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace cerberus.Models.edmx
{
    using cerberus.Models.Reports;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    [MetadataType(typeof(FactorySiteMetadata))]
    public partial class FactorySite
    {
        
        public partial class FactorySiteMetadata
        {

            public int id { get; set; }

            [Required(ErrorMessage = "Поле наименования обязательно для заполнения")]
            [Display(Name = "Наименование")]
            [StringLength(50, MinimumLength = 2, ErrorMessage = "Длинна имени должна быть в пределах от {2} до {1} символов.")]
            //[RegularExpression("^[A-Za-z]+$", ErrorMessage = "Имя содержит недопустимые символы")]
            public string name { get; set; }

            [Required(ErrorMessage = "Отдел не выбран")]
            [Range(1, int.MaxValue, ErrorMessage = "Please select a valid department_id")]

            public int department_id { get; set; }

        }

        public static async Task<(List<IError>, List<IWarning>)> get_state(CerberusDBEntities db, int factorysite_id)
        {
            var bag = (Errors: new List<IError>(), Warnings: new List<IWarning>());
            bag.Warnings = bag.Warnings.Union(FSWorkShiftReport.get_warnings(db, factorysite_id)).ToList();
            if ((await get_unaccounted_losses(db, factorysite_id)).Count() > 0) {
                bag.Errors.Add(new UnaccountedLossesError());
            }
            return bag;
        }



        public static async Task<Dictionary<int, int>> get_unaccounted_losses(CerberusDBEntities db, int factorysite_id)
        {
            FactorySite fs = db.FactorySites.Find(factorysite_id);

            var raw_remains = FSWorkShiftReport.get_reports_with_remains(db, factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.remains)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);
            var raw_production_costs = await ProductionRegistry.get_costs(db,db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString() && p.department_id == fs.department_id).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.factorysite_id == factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.produced)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value));


            var raw_losses = FSWorkShiftReport.get_reports_with_losses(db, factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.losses)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);

            var raw_consumed = Report.time_filter_last_day(FSSupplyRequirementReport.get_satisfied_fs(db,factorysite_id).Select(r => (Report)r).ToList()).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, ((FSSupplyRequirementReport)p).items)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);

            return misc.MergeDictionariesWithSum(
                misc.MergeDictionariesWithSum(
                    misc.MergeDictionariesWithSum(raw_production_costs, raw_losses),
                    raw_remains
                    ).ToDictionary(kv => kv.Key, kv => -kv.Value),
                raw_consumed
                ).Where(kv => kv.Value > 0).ToDictionary(kv => kv.Key, kv => kv.Value);


        }

        class UnaccountedLossesError : IError
        {
            string text_message;
            string html_message;
            public UnaccountedLossesError()
            {
                text_message = "Выявлена незадекларированная потеря РМ (несоответствие отчетов)";
                html_message =
                    "<p>Выявлена незадекларированная потеря РМ (несоответствие отчетов)</p>";

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
