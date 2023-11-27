using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Npgsql;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{


    [ProvideMenu]

    public class ProductionRegistriesController : Controller
    {
        private CerberusDBEntities _db;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;
        private IItemsRegistryService _itemRegistryService;

        public ProductionRegistriesController(
            CerberusDBEntities db,
            ApplicationUserManager userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            IItemsRegistryService itemRegistryService
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _itemRegistryService = itemRegistryService;
        }

        // GET: ProductionRegistries
        public async Task<ActionResult> Index()
        {
            var products = _db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionItemViewModel
                {
                    production_item = _mapper.Map<ItemViewModel>(pr.Key),
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => _mapper.Map<ItemViewModel>(p.ItemsRegistry1), p => p.count)
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

            if (!_db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }

            ProductionItemViewModel item = _db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionItemViewModel
                {
                    production_item = _mapper.Map<ItemViewModel>(pr.Key),
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => _mapper.Map<ItemViewModel>(p.ItemsRegistry1), p => p.count)
                }).First();


            return View(item);
        }

        // GET: ProductionRegistries/Create
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.ItemVariants = new SelectList(_itemRegistryService.get_variants(), "id", "name");
            return View();
        }

        // POST: ProductionRegistries/Create


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductionItemCreateModel model)
        {
            if (ModelState.IsValid)
            {

                var conn = (NpgsqlConnection)_db.Database.Connection;
                conn.Open();


                foreach (var p1 in model.requirement_ids)
                {
                    ProductionRegistry item = new ProductionRegistry();

                    item.production_id = model.production_id;
                    item.requirement_id = Convert.ToInt32(p1.Key);
                    item.count = Convert.ToInt32(p1.Value);

                    using (var cmd = new NpgsqlCommand("INSERT INTO \"public\".\"ProductionRegistry\" (\"production_id\", \"requirement_id\", \"count\") VALUES (@ProductionId, @RequirementId, @Count) RETURNING \"id\"", conn))
                    {
                        cmd.Parameters.AddWithValue("ProductionId", model.production_id);
                        cmd.Parameters.AddWithValue("RequirementId", Convert.ToInt32(p1.Key));
                        cmd.Parameters.AddWithValue("Count", Convert.ToInt32(p1.Value));

                        var insertedId = cmd.ExecuteScalar();

                    }

                }
                conn.Close();

                ViewBag.ItemVariants = new SelectList(_itemRegistryService.get_variants(), "id", "name");
                return RedirectToAction("Index", "Home");
            }


            return View(model);
        }

        // GET: ProductionRegistries/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!_db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }
            ProductionItemEditModel item = _db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionItemEditModel
                {
                    production_item = _mapper.Map<ItemViewModel>(pr.Key),
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => _mapper.Map<ItemViewModel>(p.ItemsRegistry1), p => p.count)
                }).First();
            return View(item);
        }

        // POST: ProductionRegistries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductionItemEditModel model)
        {
            if (ModelState.IsValid)
            {

                var conn = (NpgsqlConnection)_db.Database.Connection;
                conn.Open();

                using (var cmd = new NpgsqlCommand("DELETE FROM \"public\".\"ProductionRegistry\" WHERE \"production_id\" = @ProductionId", conn))
                {
                    cmd.Parameters.AddWithValue("ProductionId", model.production_id);

                    var insertedId = cmd.ExecuteNonQuery();

                }

                foreach (var p1 in model.requirement_ids)
                {
                    ProductionRegistry item = new ProductionRegistry();

                    item.production_id = model.production_id;
                    item.requirement_id = Convert.ToInt32(p1.Key);
                    item.count = Convert.ToInt32(p1.Value);

                    using (var cmd = new NpgsqlCommand("INSERT INTO \"public\".\"ProductionRegistry\" (\"production_id\", \"requirement_id\", \"count\") VALUES (@ProductionId, @RequirementId, @Count) RETURNING \"id\"", conn))
                    {
                        cmd.Parameters.AddWithValue("ProductionId", model.production_id);
                        cmd.Parameters.AddWithValue("RequirementId", Convert.ToInt32(p1.Key));
                        cmd.Parameters.AddWithValue("Count", Convert.ToInt32(p1.Value));

                        var insertedId = cmd.ExecuteScalar();

                    }

                }
                conn.Close();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ProductionRegistries/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!_db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }
            ProductionItemViewModel item = _db.ProductionRegistries
                .Include(p => p.ItemsRegistry)
                .Include(p => p.ItemsRegistry1).Where(p => p.production_id == id).GroupBy(p => p.ItemsRegistry).ToList().Select(pr => new ProductionItemViewModel
                {
                    production_item = _mapper.Map<ItemViewModel>(pr.Key),
                    requirement_items = pr.Select(p => new { p.ItemsRegistry1, p.count }).ToDictionary(p => _mapper.Map<ItemViewModel>(p.ItemsRegistry1), p => p.count)
                }).First();
            return View(item);
        }

        // POST: ProductionRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!_db.ItemsRegistries.Any(p => p.id == id)) { return RedirectToAction("Index"); }

            var conn = (NpgsqlConnection)_db.Database.Connection;
            conn.Open();

            using (var cmd = new NpgsqlCommand("DELETE FROM \"public\".\"ProductionRegistry\" WHERE \"production_id\" = @ProductionId", conn))
            {
                cmd.Parameters.AddWithValue("ProductionId", id);

                var insertedId = cmd.ExecuteNonQuery();

            }
            conn.Close();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}