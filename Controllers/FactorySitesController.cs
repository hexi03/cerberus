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
    public class FactorySitesController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();

        // GET: FactorySites
        [HttpGet]
        public async Task<ActionResult> Index(int id)
        {
            var factorySites = db.FactorySites.Include(f => f.Department).Where(w => w.department_id == id);
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            return View(await factorySites.ToListAsync());
        }

        // GET: FactorySites/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactorySite factorySite = await db.FactorySites.FindAsync(id);
            if (factorySite == null)
            {
                return HttpNotFound();
            }
            return View(factorySite);
        }

        // GET: FactorySites/Create
        public ActionResult Create(int id)
        {
            ViewBag.department_id = new SelectList(db.Departments, "id", "name");
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            return View();
        }

        // POST: FactorySites/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "name,department_id")] FactorySite factorySite, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                
                db.FactorySites.Add(factorySite);
                await db.SaveChangesAsync();
                ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            
            //ViewBag.department_id = new SelectList(db.Departments, "id", "name", factorySite.department_id);
            // return Redirect(Request.QueryString["returnUrl"]);
            //return RedirectToAction("Index", "FactorySites", new { id = factorySite.department_id });
            return RedirectToAction("Index", "Home");
        }

        // GET: FactorySites/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactorySite factorySite = await db.FactorySites.FindAsync(id);
            if (factorySite == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(db.Departments, "id", "name", factorySite.department_id);
            return View(factorySite);
        }

        // POST: FactorySites/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,name,department_id")] FactorySite factorySite, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(factorySite).State = EntityState.Modified;
                await db.SaveChangesAsync();
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                else
                    return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            ViewBag.department_id = new SelectList(db.Departments, "id", "name", factorySite.department_id);
            return RedirectToAction("Index", "Home");
        }

        // GET: FactorySites/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FactorySite factorySite = await db.FactorySites.FindAsync(id);
            if (factorySite == null)
            {
                return HttpNotFound();
            }
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
            return View(factorySite);
        }

        // POST: FactorySites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id, string returnUrl)
        {
            FactorySite factorySite = await db.FactorySites.FindAsync(id);
            db.FactorySites.Remove(factorySite);
            await db.SaveChangesAsync();
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];
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
