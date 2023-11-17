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
using cerberus.DTO;
using Microsoft.Ajax.Utilities;
using System.Security.Cryptography;

namespace cerberus.Controllers
{


    [ProvideMenu]

    public class ProductionRegistriesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();

        // GET: ProductionRegistries
        public async Task<ActionResult> Index()
        {
            var products = db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionRegistryItem
                {
                    production_item = pr.Key,
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => p.ItemsRegistry1, p => p.count)
                }).ToList();
                

            return View(products);
        }

        // GET: ProductionRegistries/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }

            ProductionRegistryItem item = db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionRegistryItem
                {
                    production_item = pr.Key,
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => p.ItemsRegistry1, p => p.count)
                }).First();


            return View(item);
        }

        // GET: ProductionRegistries/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View(new ProductionRegistryItem { 
                
            });
        }

        // POST: ProductionRegistries/Create


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductionRegistryItem productionRegistryItem)
        {
            if (ModelState.IsValid)
            {
                if (!(db.ItemsRegistries
                    .Any(p => p.id == productionRegistryItem.production_id))) {
                    return RedirectToAction("Index");
                }

                if (!productionRegistryItem.requirement_ids.Select(p => (Key: Convert.ToInt32(p.Key),Value: Convert.ToInt32(p.Value))).All(p1 => db.ItemsRegistries.Any(p => p.id == p1.Key))) { return RedirectToAction("Index"); }
                foreach (var p1 in productionRegistryItem.requirement_ids)
                {
                    ProductionRegistry item = new ProductionRegistry();

                    item.production_id = productionRegistryItem.production_id;
                    item.requirement_id = Convert.ToInt32(p1.Key);
                    item.count = Convert.ToInt32(p1.Value);
                    
                    db.ProductionRegistries.Add(item);
                }
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            productionRegistryItem.requirement_items = db.ItemsRegistries.ToDictionary(p => p, p => 1);
            return View(productionRegistryItem);
        }

        // GET: ProductionRegistries/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }
            ProductionRegistryItem item = db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionRegistryItem
                {
                    production_item = pr.Key,
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => p.ItemsRegistry1, p => p.count)
                }).First();
            return View(item);
        }

        // POST: ProductionRegistries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductionRegistryItem productionRegistryItem)
        {
            if (ModelState.IsValid)
            {
                if (!db.ItemsRegistries.Any(p => p.id == productionRegistryItem.production_id)) { return RedirectToAction("Index"); }
                if (!productionRegistryItem.requirement_ids.Select(p => (Key: Convert.ToInt32(p.Key), Value: Convert.ToInt32(p.Value))).All(p1 => db.ItemsRegistries.Any(p => p.id == p1.Key))) { return RedirectToAction("Index"); }

                db.ProductionRegistries.Where(p => p.production_id == productionRegistryItem.production_id).ForEach(p =>
                {
                    db.ProductionRegistries.Remove(p);
                });
                await db.SaveChangesAsync();
                productionRegistryItem.requirement_ids.Select(p => (Key: Convert.ToInt32(p.Key), Value: Convert.ToInt32(p.Value))).ForEach(p1 =>
                    db.ProductionRegistries.Add(new ProductionRegistry
                    {
                        production_id = productionRegistryItem.production_id,
                        requirement_id = p1.Key,
                        count = Convert.ToInt32(p1.Value)
                    })
                );
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(productionRegistryItem);
        }

        // GET: ProductionRegistries/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }
            ProductionRegistryItem item = db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionRegistryItem
                {
                    production_item = pr.Key,
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => p.ItemsRegistry1, p => p.count)
                }).First();
            return View(item);
        }

        // POST: ProductionRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }
            
            db.ProductionRegistries.Where(p => p.production_id == id).ForEach(p =>
            {
                db.ProductionRegistries.Remove(p);
            });
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