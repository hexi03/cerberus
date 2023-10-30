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
    public class ItemsRegistriesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();

        // GET: ItemsRegistries
        public async Task<ActionResult> Index()
        {
            return View(await db.ItemsRegistries.ToListAsync());
        }

        // GET: ItemsRegistries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemsRegistry itemsRegistry = await db.ItemsRegistries.FindAsync(id);
            if (itemsRegistry == null)
            {
                return HttpNotFound();
            }
            return View(itemsRegistry);
        }

        // GET: ItemsRegistries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ItemsRegistries/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,Amount,Batch")] ItemsRegistry itemsRegistry)
        {
            if (ModelState.IsValid)
            {
                db.ItemsRegistries.Add(itemsRegistry);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(itemsRegistry);
        }

        // GET: ItemsRegistries/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemsRegistry itemsRegistry = await db.ItemsRegistries.FindAsync(id);
            if (itemsRegistry == null)
            {
                return HttpNotFound();
            }
            return View(itemsRegistry);
        }

        // POST: ItemsRegistries/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Amount,Batch")] ItemsRegistry itemsRegistry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemsRegistry).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(itemsRegistry);
        }

        // GET: ItemsRegistries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemsRegistry itemsRegistry = await db.ItemsRegistries.FindAsync(id);
            if (itemsRegistry == null)
            {
                return HttpNotFound();
            }
            return View(itemsRegistry);
        }

        // POST: ItemsRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ItemsRegistry itemsRegistry = await db.ItemsRegistries.FindAsync(id);
            db.ItemsRegistries.Remove(itemsRegistry);
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
