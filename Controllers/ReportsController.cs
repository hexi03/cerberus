using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cerberus.Models;
using cerberus.Models.Reports;
using Microsoft.AspNet.Identity;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using cerberus.Models.edmx;
using cerberus.DTO.Reports;
using System.Xml.Linq;
using Microsoft.Owin.Security.Twitter.Messages;
using System.Text.Json;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class ReportsController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: Reports/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = (await db.Reports.FindAsync(id)).from_generic();

            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }



        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReplenishmentReport(int id)
        {
            ViewBag.warehouse_id = id;
            return View("~/Views/Shared/Reports/WHReplenishmentReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHInventarisationReport(int id)
        {
            ViewBag.warehouse_id = id;
            return View("~/Views/Shared/Reports/WHInventarisationReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReleaseReport(int id)
        {
            var department_id = (await db.WareHouses.FindAsync(id)).department_id;
            ViewBag.warehouse_id = id;
            ViewBag.SupplyRequirementReportVariants = FSSupplyRequirementReport.get_unsatisfied(db, department_id).ToList();

            return View("~/Views/Shared/Reports/WHReleaseReportForm.cshtml");
        }

        [WareHouseAuthorize]

        public async Task<ActionResult> CreateWHShipmentReport(int id)
        {

            ViewBag.warehouse_id = id;
            return View("~/Views/Shared/Reports/WHShipmentReportForm.cshtml");
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> CreateWHReplenishmentWorkShiftReport(int id)
        {
            var department_id = (await db.WareHouses.FindAsync(id)).department_id;
            ViewBag.warehouse_id = id;
            ViewBag.WorkShiftReportVariants = FSWorkShiftReport.get_unsatisfied(db,department_id).ToList();
            return View("~/Views/Shared/Reports/WHReplenishmentWorkShiftReportForm.cshtml");
        }


        [FactorySiteAuthorize]
        public async Task<ActionResult> CreateFSWorkShiftReport(int id)
        {
            ViewBag.factorysite_id = id;

            ViewBag.WHVariants = new SelectList(await FactorySiteWareHouseClaim.get_warehouses(ViewBag.factorysite_id), "id", "name");
            return View("~/Views/Shared/Reports/FSWorkShiftReportForm.cshtml");
        }

        [FactorySiteAuthorize]
        public async Task<ActionResult> CreateFSSupplyRequirementReport(int id)
        {
            ViewBag.factorysite_id = id;

            ViewBag.WHVariants = new SelectList(await FactorySiteWareHouseClaim.get_warehouses(ViewBag.factorysite_id), "id", "name");
            return View("~/Views/Shared/Reports/FSSupplyRequirementReportForm.cshtml");
        }



        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHReplenishmentReport(WHReplenishmentReportFormDTO report)
        {
            
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var warehouse_list = GroupWareHouseClaim.get_group_warehouses(db, group_ids);

            if (!warehouse_list.Any(e => e.id == report.warehouse_id)) {
                return RedirectToAction("Index", "Home");
            }

            

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHReplenishment.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                Report.save(db, WHReplenishmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            
            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHInventarisationReport(WHInventarisationReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var warehouse_list = GroupWareHouseClaim.get_group_warehouses(db, group_ids);

            if (!warehouse_list.Any(e => e.id == report.warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }


            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHInventarisation.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                Report.save(db, WHInventarisationReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHReleaseReport(WHReleaseReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var warehouse_list = GroupWareHouseClaim.get_group_warehouses(db, group_ids);

            if (!warehouse_list.Any(e => e.id == report.warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!db.Reports.Any(e => e.id == report.supply_requirement_id))
            {
                return RedirectToAction("Index", "Home");
            }

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHRelease.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                Report.save(db, WHReleaseReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHShipmentReport(WHShipmentReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var warehouse_list = GroupWareHouseClaim.get_group_warehouses(db, group_ids);

            if (!warehouse_list.Any(e => e.id == report.warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }


            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHShipment.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                Report.save(db, WHShipmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> CreateWHWorkShiftReplenishmentReport(WHWorkShiftReplenishmentReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var warehouse_list = GroupWareHouseClaim.get_group_warehouses(db, group_ids);

            if (!warehouse_list.Any(e => e.id == report.warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHWorkShiftReplenishment.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                Report.save(db, WHWorkShiftReplenishmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> CreateFSWorkShiftReport(FSWorkShiftReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorisites_list = GroupFactorySiteClaim.get_group_factorysites(db, group_ids);

            if (!factorisites_list.Any(e => e.id == report.factorysite_id))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!(await FactorySiteWareHouseClaim.get_warehouses(report.factorysite_id)).Any(e => e.id == report.target_warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }

            var department_id = (await db.FactorySites.FindAsync(report.factorysite_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.FSWorkShift.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                Report.save(db, FSWorkShiftReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> CreateFSSupplyRequirementReport(FSSupplyRequirementReportFormDTO report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorisites_list = GroupFactorySiteClaim.get_group_factorysites(db, group_ids);

            if (!factorisites_list.Any(e => e.id == report.factorysite_id))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!(await FactorySiteWareHouseClaim.get_warehouses(report.factorysite_id)).Any(e => e.id == report.target_warehouse_id))
            {
                return RedirectToAction("Index", "Home");
            }

            var department_id = (await db.FactorySites.FindAsync(report.factorysite_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.FSSupplyRequirement.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                Report.save(db, FSSupplyRequirementReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(report);
        }


        public async Task<ActionResult> GetFSWorkShiftReportUnsatisfiedItemList(int id)
        {
            return View(ItemsRegistry.get_list(db, FSWorkShiftReport.get_unsatisfied_item_list(db, id)));
        }

        public async Task<ActionResult> GetFSSupplyRequirementUnsatisfiedItemList(int id)
        {
            return View(ItemsRegistry.get_list(db, FSSupplyRequirementReport.get_unsatisfied_item_list(db, id)));
        }




        // GET: Reports/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = (await db.Reports.FindAsync(id)).from_generic();
            if (report == null)
            {
                return HttpNotFound();
            }
            
            switch ((Report.Types)Enum.Parse(typeof(Report.Types), report.report_type))
            {
                case Report.Types.WHRelease:
                    return await this.EditWHReleaseReport(WHReleaseReportFormDTO.Mapper.map((WHReleaseReport)report));
                    
                case Report.Types.WHInventarisation:
                    return await this.EditWHInventarisationReport(WHInventarisationReportFormDTO.Mapper.map((WHInventarisationReport)report));
                    
                case Report.Types.WHReplenishment:
                    return await this.EditWHReplenishmentReport(WHReplenishmentReportFormDTO.Mapper.map((WHReplenishmentReport)report));
                    
                case Report.Types.WHShipment:
                    return await this.EditWHShipmentReport(WHShipmentReportFormDTO.Mapper.map((WHShipmentReport)report));
                    
                case Report.Types.WHWorkShiftReplenishment:
                    return await this.EditWHReplenishmentWorkShiftReport(WHWorkShiftReplenishmentReportFormDTO.Mapper.map((WHWorkShiftReplenishmentReport)report));
                    
                case Report.Types.FSWorkShift:
                    return await this.EditFSWorkShiftReport(FSWorkShiftReportFormDTO.Mapper.map((FSWorkShiftReport)report));
                    
                case Report.Types.FSSupplyRequirement:
                    return await this.EditFSSupplyRequirementReport(FSSupplyRequirementReportFormDTO.Mapper.map((FSSupplyRequirementReport)report));

            }
            return RedirectToAction("Index", "Home");
        }

        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReplenishmentReport(WHReplenishmentReportFormDTO rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/WHReplenishmentReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHInventarisationReport(WHInventarisationReportFormDTO rep)
        {
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/WHInventarisationReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReleaseReport(WHReleaseReportFormDTO rep)
        {
            var department_id = (await db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            ViewBag.SupplyRequirementReportVariants = FSSupplyRequirementReport.get_unsatisfied(db, department_id).ToList();

            return View("~/Views/Shared/Reports/WHReleaseReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHShipmentReport(WHShipmentReportFormDTO rep)
        {

            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/WHShipmentReportForm.cshtml", rep);
        }

        [WareHouseAuthorize]
        [NonAction]
        public async Task<ActionResult> EditWHReplenishmentWorkShiftReport(WHWorkShiftReplenishmentReportFormDTO rep)
        {
            var department_id = (await db.WareHouses.FindAsync(rep.warehouse_id)).department_id;
            ViewBag.warehouse_id = rep.warehouse_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WorkShiftReportVariants = FSWorkShiftReport.get_unsatisfied(db, department_id).ToList();
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/WHReplenishmentWorkShiftReportForm.cshtml", rep);
        }


        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> EditFSWorkShiftReport(FSWorkShiftReportFormDTO rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.produced.Keys.Union(rep.losses.Keys.Union(rep.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WHVariants = new SelectList(await FactorySiteWareHouseClaim.get_warehouses(ViewBag.factorysite_id), "id", "name");
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/FSWorkShiftReportForm.cshtml", rep);
        }

        [FactorySiteAuthorize]
        [NonAction]
        public async Task<ActionResult> EditFSSupplyRequirementReport(FSSupplyRequirementReportFormDTO rep)
        {
            ViewBag.factorysite_id = rep.factorysite_id;
            var keys = rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            ViewBag.WHVariants = new SelectList(await FactorySiteWareHouseClaim.get_warehouses(ViewBag.factorysite_id), "id", "name");
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/FSSupplyRequirementReportForm.cshtml", rep);
        }



        // POST: Reports/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHReplenishmentReport(int id, WHReplenishmentReportFormDTO r_rep)
        {
            WHReplenishmentReportFormDTO report = new WHReplenishmentReportFormDTO();
            report.items = r_rep.items;


            var prev_rep = (WHReplenishmentReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.warehouse_id = prev_rep.warehouse_id;

            report.report_type = Report.Types.WHReplenishment.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                var rep = WHReplenishmentReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            return View("~/Views/Shared/Reports/WHReplenishmentReportForm.cshtml", r_rep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHInventarisationReport(int id, WHInventarisationReportFormDTO r_rep)
        {
            WHInventarisationReportFormDTO report = new WHInventarisationReportFormDTO();
            report.items = r_rep.items;



            var prev_rep = (WHInventarisationReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.warehouse_id = prev_rep.warehouse_id;
            report.report_type = Report.Types.WHInventarisation.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                var rep = WHInventarisationReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/WHInventarisationReportForm.cshtml", r_rep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHReleaseReport(int id, WHReleaseReportFormDTO r_rep)
        {
            WHReleaseReportFormDTO report = new WHReleaseReportFormDTO();
            report.items = r_rep.items;

            var prev_rep = (WHReleaseReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.warehouse_id = prev_rep.warehouse_id;
            report.supply_requirement_id = prev_rep.supply_requirement_id;
            report.report_type = Report.Types.WHRelease.ToString();
            report.timestamp = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                var rep = WHReleaseReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/WHReleaseReportForm.cshtml", r_rep);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHShipmentReport(int id, WHShipmentReportFormDTO r_rep)
        {
            WHShipmentReportFormDTO report = new WHShipmentReportFormDTO();
            report.items = r_rep.items;


            var prev_rep = (WHShipmentReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.warehouse_id = prev_rep.warehouse_id;
            report.report_type = Report.Types.WHShipment.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                var rep = WHShipmentReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/WHShipmentReportForm.cshtml", r_rep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize(Parameter = "warehouse_id")]
        public async Task<ActionResult> EditWHWorkShiftReplenishmentReport(int id, WHWorkShiftReplenishmentReportFormDTO r_rep)
        {
            WHWorkShiftReplenishmentReportFormDTO report = new WHWorkShiftReplenishmentReportFormDTO();
            report.items = r_rep.items;

            var prev_rep = (WHWorkShiftReplenishmentReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.warehouse_id = prev_rep.warehouse_id;
            report.workshift_id = prev_rep.workshift_id;


            report.report_type = Report.Types.WHWorkShiftReplenishment.ToString();
            report.timestamp = DateTime.Now;
            if (ModelState.IsValid)
            {
                var rep = WHWorkShiftReplenishmentReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.warehouse_id = prev_rep.warehouse_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/WHReplenishmentWorkShiftReportForm.cshtml", r_rep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> EditFSWorkShiftReport(int id, FSWorkShiftReportFormDTO r_rep)
        {
            FSWorkShiftReportFormDTO report = new FSWorkShiftReportFormDTO();
            report.produced = r_rep.produced;
            report.losses = r_rep.losses;
            report.remains = r_rep.remains;



            var prev_rep = (FSWorkShiftReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.factorysite_id = prev_rep.factorysite_id;
            report.target_warehouse_id = prev_rep.target_warehouse_id;

            report.report_type = Report.Types.FSWorkShift.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                var rep = FSWorkShiftReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.factorysite_id = prev_rep.factorysite_id;
            var keys = prev_rep.produced.Keys.Union(prev_rep.losses.Keys.Union(prev_rep.remains.Keys)).Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/FSWorkShiftReportForm.cshtml", r_rep);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize(Parameter = "factorysite_id")]
        public async Task<ActionResult> EditFSSupplyRequirementReport(int id, FSSupplyRequirementReportFormDTO r_rep)
        {
            FSSupplyRequirementReportFormDTO report = new FSSupplyRequirementReportFormDTO();
            report.items = r_rep.items;


            var prev_rep = (FSSupplyRequirementReport)(await db.Reports.FindAsync(id)).from_generic();

            report.creator_id = prev_rep.creator_id;
            report.department_id = prev_rep.department_id;
            report.factorysite_id = prev_rep.factorysite_id;
            report.target_warehouse_id = prev_rep.target_warehouse_id;

            report.report_type = Report.Types.FSSupplyRequirement.ToString();
            report.timestamp = DateTime.Now;

            if (ModelState.IsValid)
            {
                var rep = FSSupplyRequirementReport.from(report).to_generic();
                rep.id = id;
                Report.save(db, rep);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.mode = "edit";
            ViewBag.factorysite_id = prev_rep.factorysite_id;
            var keys = prev_rep.items.Keys.Select(k => Convert.ToInt32(k)).ToList();
            ViewBag.Items = db.ItemsRegistries.Where(it => keys.Contains(it.id)).ToDictionary(kv => kv.id.ToString(), kv => kv);
            return View("~/Views/Shared/Reports/FSSupplyRequirementReportForm.cshtml", r_rep);
        }

        // GET: Reports/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Report report = await db.Reports.FindAsync(id);
            if (report == null)
            {
                return HttpNotFound();
            }
            return View(report);
        }

        // POST: Reports/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Report report = await db.Reports.FindAsync(id);
            db.Reports.Remove(report);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
