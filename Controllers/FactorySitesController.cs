using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.Reports;
using cerberus.Models.ViewModels;
using cerberus.Models.ViewModels.Reports;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403]
    public class FactorySitesController : Controller
    {
        private CerberusDBEntities _db;
        private IUserService _userManager;
        private IGroupService _roleManager;
        private IMapper _mapper;
        private IFactorySiteAccessService _updateFactorySiteRolesService;
        IFactorySiteSupplyManagementService _updateFactorySiteSupplySourcesService;
        IFactorySiteService _factorySiteService;
        public FactorySitesController(
            CerberusDBEntities db,
            IUserService userManager,
            IGroupService roleManager,
            IMapper mapper,
            IFactorySiteService factorySiteService,
            IFactorySiteAccessService updateFactorySiteRolesService,
            IFactorySiteSupplyManagementService updateFactorySiteSupplySourcesService
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _factorySiteService = factorySiteService;
            _updateFactorySiteRolesService = updateFactorySiteRolesService;
            _updateFactorySiteSupplySourcesService = updateFactorySiteSupplySourcesService;
        }
        // GET: FactorySites
        [HttpGet]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Partial)]
        public async Task<ActionResult> Index(int id)
        {
            var user_id = User.Identity.GetUserId();


            List<FactorySiteViewModel> factorySites =
                (await (await _updateFactorySiteRolesService.get_user_factorysites_async(user_id))
                .Where(fs => fs.department_id == id).ToListAsync())
                .Select(w => _mapper.Map<FactorySiteViewModel>(w)).ToList();
            return View(factorySites);
        }

        // GET: FactorySites/Details/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Details(int id)
        {
            var user_id = User.Identity.GetUserId();


            FactorySiteViewModel model = _mapper.Map<FactorySiteViewModel>(await _db.FactorySites.FindAsync(id));
            ViewBag.ReportList = ( await FactorySiteReport.get_reports(_db, model.id)).ToList().Select(r => _mapper.Map<ReportViewModel>(r)).ToList();
            ViewBag.State = await _factorySiteService.get_state(id);
            ViewBag.Users = (await _userManager.GetAllUsersAsync()).ToDictionary(user => user.Id, user => _mapper.Map<ApplicationUserViewModel>(user));

            return View(model);
        }

        // GET: FactorySites/Create
        [HttpGet]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Create(int id)
        {

            return View();
        }

        // POST: FactorySites/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Create(FactorySiteCreateModel model)
        {
            var user_id = User.Identity.GetUserId();



            if (ModelState.IsValid)
            {
                var adminID = (await _roleManager.GetGroupByNameAsync("Admin")).Id;

                var group_ids = (await _userManager.GetUserGroupsByIdAsync(user_id)).Select(group => group.Id);
                var factorySite = _db.FactorySites.Add(_mapper.Map<FactorySite>(model));

                var group_id = _db.GroupDepartmentClaims.Where(e => e.department_id == factorySite.department_id).Select(e => e.group_id).Prepend(adminID).Where(e => group_ids.Contains(e)).First();

                _db.GroupFactorySiteClaims.Add(new GroupFactorySiteClaim()
                {
                    group_id = group_id,
                    factorysite_id = factorySite.id
                });

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // GET: FactorySites/Edit/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Edit(int id)
        {
            FactorySiteEditModel model = _mapper.Map<FactorySiteEditModel>(await _db.FactorySites.FindAsync(id));


            return View(model);
        }

        // POST: FactorySites/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize]
        //[DepartmentAuthorize(level = DepartmentAccessLevels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Edit(FactorySiteEditModel model)
        {

            if (ModelState.IsValid)
            {
                var department = (await _db.FactorySites.FindAsync(model.id)).Department;
                var factorySite = _mapper.Map<FactorySite>(model);
                factorySite.department_id = department.id;

                _db.FactorySites.AddOrUpdate(factorySite);

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: FactorySites/Delete/5
        [FactorySiteAuthorize]
        public async Task<ActionResult> Delete(int id)
        {
            FactorySiteViewModel model = _mapper.Map<FactorySiteViewModel>(await _db.FactorySites.FindAsync(id));

            return View(model);
        }

        // POST: FactorySites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [FactorySiteAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];

            FactorySite factorySite = await _db.FactorySites.FindAsync(id);
            _db.FactorySites.Remove(factorySite);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        public async Task<ActionResult> ManageAccess(int id)
        {

            var factorySite = await _db.FactorySites.FindAsync(id);

            ViewBag.Roles = (await _roleManager.GetAllGroupsAsync()).ToList();

            FactorySiteRolesEditModel model = _mapper.Map<FactorySiteRolesEditModel>(factorySite);
            return View(model);
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageAccess(FactorySiteRolesEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _updateFactorySiteRolesService.updatePreviligedGroups(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);

        }

        [FactorySiteAuthorize]
        public async Task<ActionResult> ManageSupply(int id)
        {

            var factorySite = await _db.FactorySites.FindAsync(id);

            var department = factorySite.Department;

            ViewBag.WareHouses = _db.WareHouses.Where(e => e.department_id == department.id).ToList();

            FactorySiteSupplyEditModel model = _mapper.Map<FactorySiteSupplyEditModel>(factorySite);
            return View(model);
        }


        [Authorize403(Roles = "Admin")]
        [FactorySiteAuthorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageSupply(FactorySiteSupplyEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _updateFactorySiteSupplySourcesService.updateSupplySources(model);
                return RedirectToAction("Index", "Home");
            }
            return View(model);

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}