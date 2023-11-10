﻿using System;
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

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class ReportsController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        // GET: Reports
        public async Task<ActionResult> Index()
        {
            return View(await db.Reports.ToListAsync());
        }

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

        /*
        public async Task<ActionResult> CreateMWorkPlanReport(int id)
        {
            ViewBag.department_id = id;
            ViewBag.FSVariants = new SelectList(db.FactorySites, "id", "name");
            return View("~/Views/Shared/Reports/MWorkPlanReportForm.cshtml");
        }

        public async Task<ActionResult> CreateMProducementsReport(int id)
        {
            ViewBag.department_id = id;
            ViewBag.WHVariants = new SelectList(db.WareHouses, "id", "name");
            return View("~/Views/Shared/Reports/MProducementsReportForm.cshtml");
        }

        public async Task<ActionResult> CreateMSalesReport(int id)
        {
            ViewBag.department_id = id;
            ViewBag.WHVariants = new SelectList(db.WareHouses, "id", "name");
            return View("~/Views/Shared/Reports/MSalesReportForm.cshtml");
        }

        public async Task<ActionResult> CreateMInventarisationPlanReport(int id)
        {
            ViewBag.department_id = id;
            ViewBag.WHVariants = new SelectList(db.WareHouses, "id", "name");
            return View("~/Views/Shared/Reports/MInventarisationPlanReportForm.cshtml");
        }
        */










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
                return RedirectToAction("Index");
            }

            

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHReplenishment.ToString();

            if (ModelState.IsValid)
            {
                Report.save(db, WHReplenishmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }


            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHInventarisation.ToString();
            if (ModelState.IsValid)
            {
                Report.save(db, WHInventarisationReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            if (!db.Reports.Any(e => e.id == report.supply_requirement_id))
            {
                return RedirectToAction("Index");
            }

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHRelease.ToString();
            if (ModelState.IsValid)
            {
                Report.save(db, WHReleaseReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }


            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHShipment.ToString();
            if (ModelState.IsValid)
            {
                Report.save(db, WHShipmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            var department_id = (await db.WareHouses.FindAsync(report.warehouse_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.WHWorkShiftReplenishment.ToString();
            if (ModelState.IsValid)
            {
                Report.save(db, WHWorkShiftReplenishmentReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }


            if (!(await FactorySiteWareHouseClaim.get_warehouses(report.factorysite_id)).Any(e => e.id == report.target_warehouse_id))
            {
                return RedirectToAction("Index");
            }

            var department_id = (await db.FactorySites.FindAsync(report.factorysite_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.FSWorkShift.ToString();
            
                
            if (ModelState.IsValid)
            {
                Report.save(db, FSWorkShiftReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            if (!(await FactorySiteWareHouseClaim.get_warehouses(report.factorysite_id)).Any(e => e.id == report.target_warehouse_id))
            {
                return RedirectToAction("Index");
            }

            var department_id = (await db.FactorySites.FindAsync(report.factorysite_id)).department_id;


            report.creator_id = user_id;
            report.department_id = department_id;
            report.report_type = Report.Types.FSSupplyRequirement.ToString();


            if (ModelState.IsValid)
            {
                Report.save(db, FSSupplyRequirementReport.from(report).to_generic());
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(report);
        }

        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMWorkPlanReport(MWorkPlanReport report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var departments_list = GroupDepartmentClaim.get_group_departments(group_ids);

            if (!departments_list.Any(e => e.id == report.department_id))
            {
                return RedirectToAction("Index");
            }


            if ((await db.FactorySites.FindAsync(report.factorysite_id)).department_id != report.department_id)
            {
                return RedirectToAction("Index");
            }

            report.creator_id = user_id;
            

            report.report_type = Report.Types.MWorkPlan.ToString();
            if (ModelState.IsValid)
            {
                Report.save(report.to_generic());
                return RedirectToAction("Index");
            }

            return View(report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMProductmentsReport(MProductmentsReport report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var departments_list = GroupDepartmentClaim.get_group_departments(group_ids);

            if (!departments_list.Any(e => e.id == report.department_id))
            {
                return RedirectToAction("Index");
            }


            if ((await db.WareHouses.FindAsync(report.warehouse_id)).department_id != report.department_id)
            {
                return RedirectToAction("Index");
            }

            report.creator_id = user_id;


            report.report_type = Report.Types.MProductments.ToString();
            if (ModelState.IsValid)
            {
                Report.save(report.to_generic());
                return RedirectToAction("Index");
            }

            return View(report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMSalesReport(MSalesReport report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var departments_list = GroupDepartmentClaim.get_group_departments(group_ids);

            if (!departments_list.Any(e => e.id == report.department_id))
            {
                return RedirectToAction("Index");
            }


            if ((await db.WareHouses.FindAsync(report.warehouse_id)).department_id != report.department_id)
            {
                return RedirectToAction("Index");
            }

            report.creator_id = user_id;


            report.report_type = Report.Types.MSales.ToString();
            if (ModelState.IsValid)
            {
                Report.save(report.to_generic());
                return RedirectToAction("Index");
            }

            return View(report);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateMInventarisationPlanReport(MInventarisationPlanReport report)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var departments_list = GroupDepartmentClaim.get_group_departments(group_ids);

            if (!departments_list.Any(e => e.id == report.department_id))
            {
                return RedirectToAction("Index");
            }


            if ((await db.WareHouses.FindAsync(report.warehouse_id)).department_id != report.department_id)
            {
                return RedirectToAction("Index");
            }

            report.creator_id = user_id;


            report.report_type = Report.Types.MInventarisationPlan.ToString();
            if (ModelState.IsValid)
            {
                Report.save(report.to_generic());
                return RedirectToAction("Index");
            }

            return View(report);
        }
        */





        // GET: Reports/Edit/5
        public async Task<ActionResult> Edit(int? id)
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

        // POST: Reports/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,report_type,creator_id,serialized")] Report report)
        {
            if (ModelState.IsValid)
            {
                db.Entry(report).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(report);
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
            return RedirectToAction("Index");
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
