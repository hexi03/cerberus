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

namespace cerberus.Controllers
{
    public class WarehousesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();



        // GET: Warehouses
        [HttpGet]
        public async Task<ActionResult> Index(int id)
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            var wareHouses = db.WareHouses.Include(w => w.Department).Where(w => w.department_id == id);
            return View(await wareHouses.ToListAsync());
        }

        // GET: Warehouses/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = await db.WareHouses.FindAsync(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        // GET: Warehouses/Create
        [HttpGet]
        public ActionResult Create(int id)
        {
            //ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(db.Departments, "id", "name");
            return View();
        }

        // POST: Warehouses/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "name,department_id")] Warehouse warehouse, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.WareHouses.Add(warehouse);
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
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = await db.WareHouses.FindAsync(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(db.Departments, "id", "name", warehouse.department_id);
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,name,department_id")] Warehouse warehouse, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.department_id = new SelectList(db.Departments, "id", "name", warehouse.department_id);
            return RedirectToAction("Index", "Home");
        }

        // GET: Warehouses/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = await db.WareHouses.FindAsync(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];

            Warehouse warehouse = await db.WareHouses.FindAsync(id);
            db.WareHouses.Remove(warehouse);
            await db.SaveChangesAsync();
            if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
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
