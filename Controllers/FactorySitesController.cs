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
using cerberus.Models.edmx;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using cerberus.DTO;
using cerberus.Models.Reports;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class FactorySitesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        // GET: FactorySites
        [HttpGet]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Partial)]
        public async Task<ActionResult> Index(int id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            var factorySites = GroupFactorySiteClaim.get_group_factorysites(db,group_ids).Where(w => w.department_id == id).ToList();
            return View(factorySites);
        }

        // GET: FactorySites/Details/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Details(int id)
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;

            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            FactorySite factorySite = GroupFactorySiteClaim.get_group_factorysites(db,group_ids).Where(fs => fs.id == id).First();
            ViewBag.ReportList = await FactorySiteReport.get_reports(db, factorySite.id);
            return View(factorySite);
        }

        // GET: FactorySites/Create
        [HttpGet]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> Create(int id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            var departments_list = GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full);

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(departments_list, "id", "name");
            return View();
        }

        // POST: FactorySites/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Create([Bind(Include = "name,department_id")] FactorySite factorySite, string returnUrl)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            if (ModelState.IsValid)
            {
                factorySite = db.FactorySites.Add(factorySite);
                
                var group_name = db.GroupDepartmentClaims.Where(e => e.department_id == factorySite.department_id).Select(e => e.group_id).Prepend("Admin").Where(e => group_ids.Contains(e)).First();
                var group_id = (await roleManager.FindByNameAsync(group_name)).Id;
                db.GroupFactorySiteClaims.Add(new GroupFactorySiteClaim()
                {
                    group_id = group_id,
                    factorysite_id = factorySite.id
                });
                
                await db.SaveChangesAsync();

                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            return View(factorySite);
        }

        // GET: FactorySites/Edit/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Edit(int id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            FactorySite factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).Where(fs => fs.id == id).First();

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full), "id", "name", factorySite.department_id);
            return View(factorySite);
        }

        // POST: FactorySites/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Edit([Bind(Include = "id, name,department_id")] FactorySite factorySite, string returnUrl)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            if (ModelState.IsValid)
            {
                db.Entry(factorySite).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.department_id = new SelectList(GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full), "id", "name", factorySite.department_id);
            return View(factorySite);
        }

        // GET: FactorySites/Delete/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Delete(int? id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];

            FactorySite factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).Where(fs => fs.id == id).First();

            return View(factorySite);
        }

        // POST: FactorySites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            FactorySite factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).Where(fs => fs.id == id).First();
            db.FactorySites.Remove(factorySite);
            await db.SaveChangesAsync();
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        public async Task<ActionResult> ManageAccess(int id)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).FirstOrDefault(p => p.id == id);

            ViewBag.Roles = roleManager.Roles.ToList();

            return View(new FactorySiteRolesDTO
            {
                factorySite = factorySite,
                Roles = db.GroupFactorySiteClaims
                    .Where(p => p.factorysite_id == id)
                    .ToList()
                    .Select(p => ( Value: roleManager.FindById(p.group_id).Name, Key: Guid.NewGuid()))
                    .ToDictionary(kv => kv.Key.ToString(), kv => kv.Value),
            });
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageAccess(int id, Dictionary<string, string> Roles)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).FirstOrDefault(p => p.id == id);

            db.GroupFactorySiteClaims.RemoveRange(db.GroupFactorySiteClaims.Where(p => p.factorysite_id == id));

            if (Roles != null)
            {
                foreach (var r in Roles)
                {
                    db.GroupFactorySiteClaims.Add(new GroupFactorySiteClaim
                    {
                        factorysite_id = id,
                        group_id = (await roleManager.FindByNameAsync(r.Value)).Id
                    });
                }
            }

            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = factorySite.department_id });
        }

        [FactorySiteAuthorize]
        public async Task<ActionResult> ManageSupply(int id)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).FirstOrDefault(p => p.id == id);

            var department = await GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full).Where(e => e.id == factorySite.department_id).FirstOrDefaultAsync();

            ViewBag.WareHouses = db.WareHouses.Where(e => e.department_id == department.id).ToList();


            return View(new FactorySiteSupplyDTO {
                factorySite = factorySite,
                warehouses = db.FactorySiteWareHouseClaims.Where(e => e.factorysite_id == factorySite.id).Select(e => e.Warehouse).ToList(),
            });
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageSupply(int id, Dictionary<string, string> warehouse_ids)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var factorySite = GroupFactorySiteClaim.get_group_factorysites(db, group_ids).FirstOrDefault(p => p.id == id);

            var department = await GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full).Where(e => e.id == factorySite.department_id).FirstOrDefaultAsync();


            db.FactorySiteWareHouseClaims.RemoveRange(db.FactorySiteWareHouseClaims.Where(e => e.factorysite_id == id));

            foreach (var wh_id in warehouse_ids) {
                db.FactorySiteWareHouseClaims.Add(new FactorySiteWareHouseClaim()
                {
                    warehouse_id = Convert.ToInt32(wh_id.Value),
                    factorysite_id = id
                });
            }
            await db.SaveChangesAsync();
            
            return RedirectToAction("Index", new { id = factorySite.department_id });
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