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

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class WarehousesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));


        // GET: Warehouses
        [HttpGet]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Partial)]
        public async Task<ActionResult> Index(int id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            var wareHouses = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(w => w.department_id == id).ToList();
            return View(wareHouses);
        }

        // GET: Warehouses/Details/5
        [WareHouseAuthorize]
        public async Task<ActionResult> Details(int id)
        {


            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;

            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            Warehouse warehouse = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(wh => wh.id == id).ToList().First();

            return View(warehouse);
        }

        // GET: Warehouses/Create
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

        // POST: Warehouses/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Create([Bind(Include = "id,name,department_id")] Warehouse warehouse, string returnUrl)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            if (ModelState.IsValid)
            {
                warehouse = db.WareHouses.Add(warehouse);
                
                var group_name = db.GroupDepartmentClaims.Where(e => e.department_id == warehouse.department_id).Select(e => e.group_id).Prepend("Admin").Where(e => group_ids.Contains(e)).First();
                var group_id = (await roleManager.FindByNameAsync(group_name)).Id;
                db.GroupWareHouseClaims.Add(new GroupWareHouseClaim()
                {
                    group_id = group_id,
                    warehouse_id = warehouse.id
                });
                
                await db.SaveChangesAsync();
                //return RedirectToAction("Index", "Warehouses", new { id = warehouse.department_id });

                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }

            //ViewBag.department_id = new SelectList(db.Departments, "id", "name", warehouse.department_id);
            //return RedirectToAction("Index", "Warehouses", new { id = warehouse.department_id });
            return RedirectToAction("Index", "Home");
        }

        // GET: Warehouses/Edit/5
        [WareHouseAuthorize]
        public async Task<ActionResult> Edit(int id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            Warehouse warehouse = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(wh => wh.id == id).First();

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full), "id", "name", warehouse.department_id);
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize]
        public async Task<ActionResult> Edit([Bind(Include = "id,name,department_id")] Warehouse warehouse, string returnUrl)
        {

            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            if (ModelState.IsValid)
            {
                db.Entry(warehouse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.department_id = new SelectList(GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full), "id", "name", warehouse.department_id);
            return RedirectToAction("Index", "Home");
        }

        // GET: Warehouses/Delete/5
        [WareHouseAuthorize]
        public async Task<ActionResult> Delete(int? id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];


            Warehouse warehouse = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(wh => wh.id == id).First();

            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            var user_id = User.Identity.GetUserId();
            
            var group_ids = await userManager.GetRolesAsync(user_id);

            Warehouse warehouse = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(wh => wh.id == id).First();
            db.WareHouses.Remove(warehouse);
            await db.SaveChangesAsync();
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction("Index", "Home");
        }


        [Authorize403(Roles = "Admin")]
        [WareHouseAuthorize]
        public async Task<ActionResult> ManageAccess(int id) {

            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var wh = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(p => p.id == id).First();

            ViewBag.Roles = roleManager.Roles.ToList();

            
            return View(new WareHouseRolesDTO {
                warehouse = wh,
                Roles = db.GroupWareHouseClaims.Where(p => p.warehouse_id == id).ToList().Select(p => roleManager.FindById(p.group_id).Name).ToDictionary(kv => kv, kv => Guid.NewGuid().ToString()),
            });
        }
        [Authorize403(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize]
        public async Task<ActionResult> ManageAccess(int id, Dictionary<string, string> Roles)
        {
            
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var wh = GroupWareHouseClaim.get_group_warehouses(db, group_ids).Where(p => p.id == id).First();

            db.GroupWareHouseClaims.RemoveRange(db.GroupWareHouseClaims.Where(p => p.warehouse_id == id));

            if (Roles != null)
            {
                foreach (var r in Roles)
                {
                    db.GroupWareHouseClaims.Add(new GroupWareHouseClaim()
                    {
                        warehouse_id = id,
                        group_id = (await roleManager.FindByNameAsync(r.Value)).Id
                    });
                }
            }
            await db.SaveChangesAsync();
            return RedirectToAction("Index", new { id = wh.department_id });
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
