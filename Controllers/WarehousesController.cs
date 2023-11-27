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
    public class WarehousesController : Controller
    {
        private CerberusDBEntities _db;
        private IUserService _userManager;
        private IGroupService _roleManager;
        private IMapper _mapper;
        private IWareHouseAccessService _updateWareHouseRolesService;
        private IItemsRegistryService _itemRegistryService;
        private IWarehouseService _warehouseService;
        public WarehousesController(
            CerberusDBEntities db,
            IUserService userManager,
            IGroupService roleManager,
            IMapper mapper,
            IWareHouseAccessService updateWareHouseRolesService,
            IItemsRegistryService itemRegistryService,
            IWarehouseService warehouseService
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _updateWareHouseRolesService = updateWareHouseRolesService;
            _itemRegistryService = itemRegistryService;
            _warehouseService = warehouseService;
        }

        [HttpGet]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Partial)]
        public async Task<ActionResult> Index(int id)
        {
            var user_id = User.Identity.GetUserId();

            List<WarehouseViewModel> wareHouses =
                (await (await _updateWareHouseRolesService.get_user_warehouses_async(user_id))
                .Where(wh => wh.department_id == id).ToListAsync())
                .Select(w => _mapper.Map<WarehouseViewModel>(w)).ToList();

            return View(wareHouses);
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> Details(int id)
        {
            var user_id = User.Identity.GetUserId();

            WarehouseViewModel model = _mapper.Map<WarehouseViewModel>(await _db.WareHouses.FindAsync(id));

            ViewBag.ReportList = (await WareHouseReport.get_reports(_db, model.id)).ToList().Select(r => _mapper.Map<ReportViewModel>(r)).ToList();
            ViewBag.State = _warehouseService.get_state(id);
            ViewBag.StorageState = _itemRegistryService.get_list(_warehouseService.get_storage_state(id)).Select(i => (_mapper.Map<ItemViewModel>(i.Item1), i.Item2 )).ToList();
            ViewBag.Users = (await _userManager.GetAllUsersAsync()).ToDictionary(user => user.Id, user => _mapper.Map<ApplicationUserViewModel>(user));

            return View(model);
        }

        [HttpGet]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Create(int id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full, Parameter = "department_id")]
        public async Task<ActionResult> Create(WarehouseCreateModel model)
        {
            var user_id = User.Identity.GetUserId();

            if (ModelState.IsValid)
            {
                var adminID = (await _roleManager.GetGroupByNameAsync("Admin")).Id;

                var group_ids = (await _userManager.GetUserGroupsByIdAsync(user_id)).Select(group => group.Id);
                var wareHouse = _db.WareHouses.Add(_mapper.Map<Warehouse>(model));

                var group_id = _db.GroupDepartmentClaims
                    .Where(e => e.department_id == wareHouse.department_id)
                    .Select(e => e.group_id)
                    .Prepend(adminID)
                    .Where(e => group_ids.Contains(e))
                    .First();

                _db.GroupWareHouseClaims.Add(new GroupWareHouseClaim()
                {
                    group_id = group_id,
                    warehouse_id = wareHouse.id
                });

                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> Edit(int id)
        {
            WarehouseEditModel model = _mapper.Map<WarehouseEditModel>(await _db.WareHouses.FindAsync(id));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize]
        public async Task<ActionResult> Edit(WarehouseEditModel model)
        {
            if (ModelState.IsValid)
            {
                var department = (await _db.WareHouses.FindAsync(model.id)).Department;
                var wareHouse = _mapper.Map<Warehouse>(model);
                wareHouse.department_id = department.id;

                _db.WareHouses.AddOrUpdate(wareHouse);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [WareHouseAuthorize]
        public async Task<ActionResult> Delete(int id)
        {
            WarehouseViewModel model = _mapper.Map<WarehouseViewModel>(await _db.WareHouses.FindAsync(id));

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [WareHouseAuthorize]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString["returnUrl"];

            Warehouse wareHouse = await _db.WareHouses.FindAsync(id);
            _db.WareHouses.Remove(wareHouse);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        [Authorize403(Roles = "Admin")]
        [WareHouseAuthorize]
        public async Task<ActionResult> ManageAccess(int id)
        {
            var wareHouse = await _db.WareHouses.FindAsync(id);

            ViewBag.Roles = (await _roleManager.GetAllGroupsAsync()).ToList();

            WareHouseRolesEditModel model = _mapper.Map<WareHouseRolesEditModel>(wareHouse);
            return View(model);
        }

        [Authorize403(Roles = "Admin")]
        [WareHouseAuthorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageAccess(WareHouseRolesEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _updateWareHouseRolesService.updatePreviligedGroups(model);
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
