using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.Reports;
using cerberus.Models.ViewModels;
using cerberus.Models.ViewModels.Reports;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403]
    public class ReportsController : Controller
    {
        private CerberusDBEntities _db;
        private IUserService _userManager;
        private IGroupService _roleManager;
        private IMapper _mapper;
        private IFactorySiteSupplyManagementService _factorySiteSupplyManagementService;
        private IItemsRegistryService _itemRegistryServices;
        private IWareHouseAccessService _wareHouseAccessService;
        private IFactorySiteAccessService factorySiteAccessService;
        public ReportsController(
            CerberusDBEntities db,
            IUserService userManager,
            IGroupService roleManager,
            IMapper mapper,
            IFactorySiteSupplyManagementService factorySiteSupplyManagementService,
            IItemsRegistryService itemRegistryServices
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _factorySiteSupplyManagementService = factorySiteSupplyManagementService;
            _itemRegistryServices = itemRegistryServices;
        }

        // GET: Reports/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = (await _db.Reports.FindAsync(id)).from_generic();
            if (report == null)
            {
                return HttpNotFound();
            }

            switch ((Report.Types)Enum.Parse(typeof(Report.Types), report.report_type))
            {
                case Report.Types.WHRelease:
                    return await this.DetailsWHReleaseReport(_mapper.Map<WHReleaseReportFormViewModel>((WHReleaseReport)report));

                case Report.Types.WHInventarisation:
                    return await this.DetailsWHInventarisationReport(_mapper.Map<WHInventarisationReportFormViewModel>((WHInventarisationReport)report));

                case Report.Types.WHReplenishment:
                    return await this.DetailsWHReplenishmentReport(_mapper.Map<WHReplenishmentReportFormViewModel>((WHReplenishmentReport)report));

                case Report.Types.WHShipment:
                    return await this.DetailsWHShipmentReport(_mapper.Map<WHShipmentReportFormViewModel>((WHShipmentReport)report));

                case Report.Types.WHWorkShiftReplenishment:
                    return await this.DetailsWHReplenishmentWorkShiftReport(_mapper.Map<WHWorkShiftReplenishmentReportFormViewModel>((WHWorkShiftReplenishmentReport)report));

                case Report.Types.FSWorkShift:
                    return await this.DetailsFSWorkShiftReport(_mapper.Map<FSWorkShiftReportFormViewModel>((FSWorkShiftReport)report));

                case Report.Types.FSSupplyRequirement:
                    return await this.DetailsFSSupplyRequirementReport(_mapper.Map<FSSupplyRequirementReportFormViewModel>((FSSupplyRequirementReport)report));

            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsWHReplenishmentReport(WHReplenishmentReportFormViewModel rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/WHReplenishmentDetails.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsWHInventarisationReport(WHInventarisationReportFormViewModel rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/WHInventarisationDetails.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsWHReleaseReport(WHReleaseReportFormViewModel rep)
        {
            var department_id = (await _db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id || r.id == rep.supply_requirement_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";

            return View("~/Views/Reports/Details/WHReleaseDetails.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsWHShipmentReport(WHShipmentReportFormViewModel rep)
        {

            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/WHShipmentDetails.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsWHReplenishmentWorkShiftReport(WHWorkShiftReplenishmentReportFormViewModel rep)
        {
            var department_id = (await _db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id || r.id == rep.workshift_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.warehouse_id).ToDictionary(kv => kv.id, kv => kv);

            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/WHReplenishmentWorkShiftDetails.cshtml", rep);
        }


        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsFSWorkShiftReport(FSWorkShiftReportFormViewModel rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.produced.Keys.Union(rep.losses.Keys.Union(rep.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.target_warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.FactorySites = _db.FactorySites.Where(r => r.id == rep.factorysite_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/FSWorkShiftDetails.cshtml", rep);
        }

        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> DetailsFSSupplyRequirementReport(FSSupplyRequirementReportFormViewModel rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.Reports = _db.Reports.Where(r => r.id == rep.id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.WareHouses = _db.WareHouses.Where(r => r.id == rep.target_warehouse_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.FactorySites = _db.FactorySites.Where(r => r.id == rep.factorysite_id).ToDictionary(kv => kv.id, kv => kv);
            ViewBag.mode = "Details";
            return View("~/Views/Reports/Details/FSSupplyRequirementDetails.cshtml", rep);
        }







        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReplenishmentReport(int id)
        {
            ViewBag.warehouse_id = id;
            return View("~/Views/Reports/Forms/WHReplenishmentReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHInventarisationReport(int id)
        {
            ViewBag.warehouse_id = id;
            return View("~/Views/Reports/Forms/WHInventarisationReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReleaseReport(int id)
        {
            var department_id = (await _db.WareHouses.FindAsync(id)).department_id;
            ViewBag.warehouse_id = id;
            ViewBag.SupplyRequirementReportVariants = FSSupplyRequirementReport.get_unsatisfied(_db, department_id).ToList();

            return View("~/Views/Reports/Forms/WHReleaseReportForm.cshtml");
        }

        [WareHouseAuthorize]

        public async Task<ActionResult> CreateWHShipmentReport(int id)
        {

            ViewBag.warehouse_id = id;
            return View("~/Views/Reports/Forms/WHShipmentReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReplenishmentWorkShiftReport(int id)
        {
            var department_id = (await _db.WareHouses.FindAsync(id)).department_id;
            ViewBag.warehouse_id = id;
            ViewBag.WorkShiftReportVariants = FSWorkShiftReport.get_unsatisfied(_db, department_id).ToList();
            return View("~/Views/Reports/Forms/WHReplenishmentWorkShiftReportForm.cshtml");
        }


        [FactorySiteAuthorize]
        public async Task<ActionResult> CreateFSWorkShiftReport(int id)
        {
            ViewBag.factorysite_id = id;

            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(ViewBag.factorysite_id), "id", "name");
            return View("~/Views/Reports/Forms/FSWorkShiftReportForm.cshtml");
        }

        [FactorySiteAuthorize]
        public async Task<ActionResult> CreateFSSupplyRequirementReport(int id)
        {
            ViewBag.factorysite_id = id;

            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(ViewBag.factorysite_id), "id", "name");
            return View("~/Views/Reports/Forms/FSSupplyRequirementReportForm.cshtml");
        }



        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHReplenishmentReport(WHReplenishmentReportFormViewModel model)
        {

            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.WareHouses.FindAsync(model.warehouse_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = WHReplenishmentReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.warehouse_id = model.warehouse_id;
            ViewBag.WorkShiftReportVariants = FSWorkShiftReport.get_unsatisfied(_db, department_id).ToList();
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHReplenishmentReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHInventarisationReport(WHInventarisationReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.WareHouses.FindAsync(model.warehouse_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = WHInventarisationReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHInventarisationReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHReleaseReport(WHReleaseReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.WareHouses.FindAsync(model.warehouse_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = WHReleaseReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.warehouse_id = model.warehouse_id;
            ViewBag.SupplyRequirementReportVariants = FSSupplyRequirementReport.get_unsatisfied(_db, department_id).ToList();
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHReleaseReportForm.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHShipmentReport(WHShipmentReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.WareHouses.FindAsync(model.warehouse_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = WHShipmentReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHShipmentReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHWorkShiftReplenishmentReport(WHWorkShiftReplenishmentReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.WareHouses.FindAsync(model.warehouse_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = WHWorkShiftReplenishmentReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHWorkShiftReplenishmentReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> CreateFSWorkShiftReport(FSWorkShiftReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.FactorySites.FindAsync(model.factorysite_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = FSWorkShiftReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.factorysite_id = model.factorysite_id;
            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(model.factorysite_id), "id", "name");
            var keys = model.produced.Keys.Union(model.losses.Keys.Union(model.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/FSWorkShiftReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> CreateFSSupplyRequirementReport(FSSupplyRequirementReportFormViewModel model)
        {
            var user_id = User.Identity.GetUserId();

            var department_id = (await _db.FactorySites.FindAsync(model.factorysite_id)).department_id;

            if (ModelState.IsValid)
            {
                var report = FSSupplyRequirementReport.from(model).to_generic();
                report.creator_id = user_id;
                report.department_id = department_id;
                
                report.timestamp = DateTime.Now;


                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.factorysite_id = model.factorysite_id;
            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(model.factorysite_id), "id", "name");
            var keys = model.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/FSSupplyRequirementReportForm.cshtml", model);
        }


        public async Task<ActionResult> GetFSWorkShiftReportUnsatisfiedItemList(int id)
        {
            return View(
                _itemRegistryServices.get_list(FSWorkShiftReport.get_unsatisfied_item_list(_db, id))
                .Select(i => (_mapper.Map<ItemViewModel>(i.Item1), i.Item2)).ToList()
                );
        }

        public async Task<ActionResult> GetFSSupplyRequirementUnsatisfiedItemList(int id)
        {
            return View(
                _itemRegistryServices.get_list(FSSupplyRequirementReport.get_unsatisfied_item_list(_db, id))
                .Select(i => (_mapper.Map<ItemViewModel>(i.Item1), i.Item2)).ToList()
                );
        }




        // GET: Reports/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = (await _db.Reports.FindAsync(id)).from_generic();
            if (report == null)
            {
                return HttpNotFound();
            }

            switch ((Report.Types)Enum.Parse(typeof(Report.Types), report.report_type))
            {
                case Report.Types.WHRelease:
                    return await this.EditWHReleaseReport(_mapper.Map<WHReleaseReportFormViewModel>((WHReleaseReport)report));

                case Report.Types.WHInventarisation:
                    return await this.EditWHInventarisationReport(_mapper.Map<WHInventarisationReportFormViewModel>((WHInventarisationReport)report));

                case Report.Types.WHReplenishment:
                    return await this.EditWHReplenishmentReport(_mapper.Map<WHReplenishmentReportFormViewModel>((WHReplenishmentReport)report));

                case Report.Types.WHShipment:
                    return await this.EditWHShipmentReport(_mapper.Map<WHShipmentReportFormViewModel>((WHShipmentReport)report));

                case Report.Types.WHWorkShiftReplenishment:
                    return await this.EditWHReplenishmentWorkShiftReport(_mapper.Map<WHWorkShiftReplenishmentReportFormViewModel>((WHWorkShiftReplenishmentReport)report));

                case Report.Types.FSWorkShift:
                    return await this.EditFSWorkShiftReport(_mapper.Map<FSWorkShiftReportFormViewModel>((FSWorkShiftReport)report));

                case Report.Types.FSSupplyRequirement:
                    return await this.EditFSSupplyRequirementReport(_mapper.Map<FSSupplyRequirementReportFormViewModel>((FSSupplyRequirementReport)report));

            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReplenishmentReport(WHReplenishmentReportFormViewModel rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/WHReplenishmentReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHInventarisationReport(WHInventarisationReportFormViewModel rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/WHInventarisationReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReleaseReport(WHReleaseReportFormViewModel rep)
        {
            var department_id = (await _db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            ViewBag.SupplyRequirementReportVariants = FSSupplyRequirementReport.get_unsatisfied(_db, department_id).ToList();

            return View("~/Views/Reports/Forms/WHReleaseReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHShipmentReport(WHShipmentReportFormViewModel rep)
        {

            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/WHShipmentReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReplenishmentWorkShiftReport(WHWorkShiftReplenishmentReportFormViewModel rep)
        {
            var department_id = (await _db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WorkShiftReportVariants = FSWorkShiftReport.get_unsatisfied(_db, department_id).ToList();
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/WHReplenishmentWorkShiftReportForm.cshtml", rep);
        }


        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> EditFSWorkShiftReport(FSWorkShiftReportFormViewModel rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.produced.Keys.Union(rep.losses.Keys.Union(rep.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(ViewBag.factorysite_id), "id", "name");
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/FSWorkShiftReportForm.cshtml", rep);
        }

        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> EditFSSupplyRequirementReport(FSSupplyRequirementReportFormViewModel rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WHVariants = new SelectList(await _factorySiteSupplyManagementService.get_warehouses(ViewBag.factorysite_id), "id", "name");
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/FSSupplyRequirementReportForm.cshtml", rep);
        }



        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHReplenishmentReport(int id, WHReplenishmentReportFormViewModel model)
        {
            Report report = WHReplenishmentReport.from(model).to_generic();

            var prev_rep = (WHReplenishmentReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            return View("~/Views/Reports/Forms/WHReplenishmentReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHInventarisationReport(int id, WHInventarisationReportFormViewModel model)
        {
            Report report = WHInventarisationReport.from(model).to_generic();

            var prev_rep = (WHInventarisationReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHInventarisationReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHReleaseReport(int id, WHReleaseReportFormViewModel model)
        {
            Report report = WHReleaseReport.from(model).to_generic();

            var prev_rep = (WHReleaseReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHReleaseReportForm.cshtml", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHShipmentReport(int id, WHShipmentReportFormViewModel model)
        {
            Report report = WHShipmentReport.from(model).to_generic();

            var prev_rep = (WHShipmentReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHShipmentReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHWorkShiftReplenishmentReport(int id, WHWorkShiftReplenishmentReportFormViewModel model)
        {
            Report report = WHWorkShiftReplenishmentReport.from(model).to_generic();

            var prev_rep = (WHWorkShiftReplenishmentReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/WHReplenishmentWorkShiftReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> EditFSWorkShiftReport(int id, FSWorkShiftReportFormViewModel model)
        {
            Report report = FSWorkShiftReport.from(model).to_generic();

            var prev_rep = (FSWorkShiftReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.factorysite_id = prev_rep.factorysite_id;
            var keys = prev_rep.produced.Keys.Union(prev_rep.losses.Keys.Union(prev_rep.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/FSWorkShiftReportForm.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> EditFSSupplyRequirementReport(int id, FSSupplyRequirementReportFormViewModel model)
        {
            Report report = FSSupplyRequirementReport.from(model).to_generic();

            var prev_rep = (FSSupplyRequirementReport)(await _db.Reports.FindAsync(id)).from_generic();

            report.id = id;
            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {

                Report.save(_db, report);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.factorysite_id = prev_rep.factorysite_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = _db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Reports/Forms/FSSupplyRequirementReportForm.cshtml", model);
        }

        // GET: Reports/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            ViewBag.Users = (await _userManager.GetAllUsersAsync()).ToDictionary(user => user.Id);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ReportViewModel report = _mapper.Map<ReportViewModel>(await _db.Reports.FindAsync(id));

            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Report report = await _db.Reports.FindAsync(id);
            _db.Reports.Remove(report);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
