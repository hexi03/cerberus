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
using System.Drawing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using cerberus.DTO;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class DepartmentsController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
        // GET: Departments

        public async Task<ActionResult> Index()
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = userManager.GetRoles(user_id);

            return View(GroupDepartmentClaim.get_group_departments(db,group_ids, GroupDepartmentClaim.Levels.Full).ToList());
        }

        // GET: Departments/Details/5
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> Details(int? id)
        {
            var user_id = User.Identity.GetUserId();
            
            var group_ids = userManager.GetRoles(user_id);

            Department department = GroupDepartmentClaim.get_group_departments(db,group_ids, GroupDepartmentClaim.Levels.Full).Where(e => e.id == id).First();

            return View(department);
        }

        // GET: Departments/Create
        [HttpGet]
        [Authorize403(Roles = "Admin")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize403(Roles = "Admin")]

        public async Task<ActionResult> Create(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> Edit(int? id)
        {

            Department department = await db.Departments.FindAsync(id);

            return View(department);
        }

        // POST: Departments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> Delete(int? id)
        {

            Department department = await db.Departments.FindAsync(id);

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize403(Roles ="Admin")]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await db.Departments.FindAsync(id);
            db.Departments.Remove(department);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        public async Task<ActionResult> ManageAccess(int id)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var department = GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full).FirstOrDefault(p => p.id == id);


            ViewBag.Roles = roleManager.Roles.ToList();

            return View(new DepartmentRolesDTO
            {
                department = department,
                Roles = db.GroupDepartmentClaims
                    .Where(p => p.department_id == id)
                    .ToList()
                    .Select(p => roleManager.FindById(p.group_id).Name)
                    .ToDictionary(kv => kv, kv => Guid.NewGuid().ToString()),
            });
        }

        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = GroupDepartmentClaim.Levels.Full)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageAccess(int id, Dictionary<string, string> Roles)
        {
            var user_id = User.Identity.GetUserId();
            var group_ids = await userManager.GetRolesAsync(user_id);

            var department = GroupDepartmentClaim.get_group_departments(db, group_ids, GroupDepartmentClaim.Levels.Full).FirstOrDefault(p => p.id == id);


            db.GroupDepartmentClaims.RemoveRange(db.GroupDepartmentClaims.Where(p => p.department_id == id));

            if (Roles != null)
            {
                foreach (var r in Roles)
                {
                    db.GroupDepartmentClaims.Add(new GroupDepartmentClaim
                    {
                        department_id = id,
                        group_id = (await roleManager.FindByNameAsync(r.Value)).Id,
                        
                    });
                }
            }

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
