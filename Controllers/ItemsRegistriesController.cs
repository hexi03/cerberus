using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403Attribute]
    public class ItemsRegistriesController : Controller
    {
        private CerberusDBEntities _db;
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IMapper _mapper;

        public ItemsRegistriesController(
            CerberusDBEntities db,
            ApplicationUserManager userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: ItemsRegistries
        public async Task<ActionResult> Index()
        {
            List<ItemViewModel> model = (await _db.ItemsRegistries.ToListAsync()).Select(i => _mapper.Map<ItemViewModel>(i)).ToList();
            return View(model);
        }

        // GET: ItemsRegistries/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemViewModel model = _mapper.Map<ItemViewModel>(await _db.ItemsRegistries.FindAsync(id));

            return View(model);
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
        public async Task<ActionResult> Create(ItemCreateModel model)
        {
            if (ModelState.IsValid)
            {
                _db.ItemsRegistries.Add(_mapper.Map<ItemsRegistry>(model));
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: ItemsRegistries/Edit/5

        public async Task<ActionResult> Edit(int? id)
        {
            ItemEditModel itemsRegistry = _mapper.Map<ItemEditModel>(await _db.ItemsRegistries.FindAsync(id));

            return View(itemsRegistry);
        }

        // POST: ItemsRegistries/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ItemsRegistry model)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(_mapper.Map<ItemsRegistry>(model)).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: ItemsRegistries/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {

            ItemViewModel model = _mapper.Map<ItemViewModel>(await _db.ItemsRegistries.FindAsync(id));

            return View(model);
        }

        // POST: ItemsRegistries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ItemsRegistry itemsRegistry = await _db.ItemsRegistries.FindAsync(id);
            _db.ItemsRegistries.Remove(itemsRegistry);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
