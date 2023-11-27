using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace cerberus.Services
{
    public interface IWareHouseAccessService
    {
        Task updatePreviligedGroups(WareHouseRolesEditModel vm);
        Task<IQueryable<Warehouse>> get_user_warehouses_async(string user_id);
        IQueryable<Warehouse> get_user_warehouses(string user_id);
    }
    public class WareHouseAccessService : IWareHouseAccessService
    {
        CerberusDBEntities _db;
        IUserService _userService;
        IGroupService _groupService;
        IDepartmentAccessService _departmentAccessService;
        public WareHouseAccessService(CerberusDBEntities db, IUserService userService, IGroupService roleService, IDepartmentAccessService departmentAccessService)
        {
            _db = db;
            _groupService = roleService;
            _userService = userService;
            _departmentAccessService = departmentAccessService;
        }
        public async Task updatePreviligedGroups(WareHouseRolesEditModel vm)
        {
            _db.GroupWareHouseClaims.RemoveRange(_db.GroupWareHouseClaims.Where(p => p.warehouse_id == vm.id));

            if (vm.Roles != null)
            {
                foreach (var r in vm.getGroupIDs())
                {
                    _db.GroupWareHouseClaims.Add(new GroupWareHouseClaim()
                    {
                        warehouse_id = vm.id,
                        group_id = (await _groupService.GetGroupByIdAsync(r)).Id
                    });
                }
            }
            await _db.SaveChangesAsync();
        }

        public async Task<IQueryable<Warehouse>> get_user_warehouses_async(string user_id)
        {

            var groups = await _userService.GetUserGroupsByIdAsync(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);

            if (group_names.Contains("Admin"))
            {
                return _db.WareHouses.Include(e => e.Department);
            }

            var deps = (await _departmentAccessService.get_user_departments_async(user_id, DepartmentAccessLevels.Full)).Select(e => e.id);

            var warehouses = _db.GroupWareHouseClaims
                .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.Warehouse.department_id)).Select(c => c.Warehouse).Include(e => e.Department);

            return warehouses;

        }

        public IQueryable<Warehouse> get_user_warehouses(string user_id)
        {
            var groups = _userService.GetUserGroupsById(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);

            if (group_names.Contains("Admin"))
            {
                return _db.WareHouses.Include(e => e.Department);
            }

            var deps = ( _departmentAccessService.get_user_departments(user_id, DepartmentAccessLevels.Full)).Select(e => e.id);

            var warehouses = _db.GroupWareHouseClaims
                .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.Warehouse.department_id)).Select(c => c.Warehouse).Include(e => e.Department);

            return warehouses;
        }
    }
}