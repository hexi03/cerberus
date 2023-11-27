using System.Linq;


namespace cerberus.Services
{
    using cerberus.Models.edmx;
    using cerberus.Models.Reports;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IFactorySiteService
    {
        Task<(List<IError>, List<IWarning>)> get_state(int factorysite_id);
        Task<Dictionary<int, int>> get_unaccounted_losses(int factorysite_id);
    }
    public partial class FactorySiteService : IFactorySiteService
    {

        CerberusDBEntities _db;
        IProductionRegistryService _productionRegistryService;

        public FactorySiteService(CerberusDBEntities db, IProductionRegistryService productionRegistryService)
        {
            _db = db;
            _productionRegistryService = productionRegistryService;
        }

        public async Task<(List<IError>, List<IWarning>)> get_state(int factorysite_id)
        {
            var bag = (Errors: new List<IError>(), Warnings: new List<IWarning>());
            bag.Warnings = bag.Warnings.Union(FSWorkShiftReport.get_warnings(_db, factorysite_id)).ToList();
            if ((await get_unaccounted_losses(factorysite_id)).Count() > 0)
            {
                bag.Errors.Add(new UnaccountedLossesError());
            }
            return bag;
        }



        public async Task<Dictionary<int, int>> get_unaccounted_losses(int factorysite_id)
        {
            FactorySite fs = _db.FactorySites.Find(factorysite_id);

            var raw_remains = FSWorkShiftReport.get_reports_with_remains(_db, factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.remains)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);
            var raw_production_costs = await _productionRegistryService.get_costs(_db.Reports.Where(p => p.report_type == Report.Types.FSWorkShift.ToString() && p.department_id == fs.department_id).ToList()
                    .Select(p => (FSWorkShiftReport)p.from_generic()).Where(p => p.factorysite_id == factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.produced)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value));


            var raw_losses = FSWorkShiftReport.get_reports_with_losses(_db, factorysite_id).Aggregate(new Dictionary<int, int>(), (acc, p) =>
                        misc.MergeDictionariesWithSum(acc, p.losses)
                    ).ToDictionary(kv => kv.Key, kv => kv.Value);

            var raw_consumed = Report.time_filter_last_day(FSSupplyRequirementReport.get_satisfied_fs(_db, factorysite_id).Select(r => (Report)r).ToList()).Aggregate(new Dictionary<int, int>(), (acc, p) =>
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
