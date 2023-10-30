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
    public class ProductionRegistriesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();

        // GET: ProductionRegistries
        public async Task<ActionResult> Index()
        {
            var productionRegistries = db.ProductionRegistries.Include(p => p.ItemsRegistry).Include(p => p.ItemsRegistry1);
            return View(await productionRegistries.ToListAsync());
        }

        // GET: ProductionRegistries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductionRegistry productionRegistry = await db.ProductionRegistries.FindAsync(id);
            if (productionRegistry == null)
            {
                return HttpNotFound();
            }
            return View(productionRegistry);
        }

        // GET: ProductionRegistries/Create
        public ActionResult Create()
        {
            ViewBag.production_id = new SelectList(db.ItemsRegistries, "Id", "Name");
            ViewBag.requirement_id = new SelectList(db.ItemsRegistries, "Id", "Name");
            return View();
        }

        // POST: ProductionRegistries/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,production_id,requirement_id")] ProductionRegistry productionRegistry)
        {
            if (ModelState.IsValid)
            {
                db.ProductionRegistries.Add(productionRegistry);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.production_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.production_id);
            ViewBag.requirement_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.requirement_id);
            return View(productionRegistry);
        }

        // GET: ProductionRegistries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductionRegistry productionRegistry = await db.ProductionRegistries.FindAsync(id);
            if (productionRegistry == null)
            {
                return HttpNotFound();
            }
            ViewBag.production_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.production_id);
            ViewBag.requirement_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.requirement_id);
            return View(productionRegistry);
        }

        // POST: ProductionRegistries/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,production_id,requirement_id")] ProductionRegistry productionRegistry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productionRegistry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.production_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.production_id);
            ViewBag.requirement_id = new SelectList(db.ItemsRegistries, "Id", "Name", productionRegistry.requirement_id);
            return View(productionRegistry);
        }

        // GET: ProductionRegistries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductionRegistry productionRegistry = await db.ProductionRegistries.FindAsync(id);
            if (productionRegistry == null)
            {
                return HttpNotFound();
            }
            return View(productionRegistry);
        }

        // POST: ProductionRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ProductionRegistry productionRegistry = await db.ProductionRegistries.FindAsync(id);
            db.ProductionRegistries.Remove(productionRegistry);
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
