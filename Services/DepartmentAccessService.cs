using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace cerberus.Services
{
    public enum DepartmentAccessLevels
    {
        Full,
        Partial
    }
    public interface IDepartmentAccessService
    {

        Task updatePreviligedGroups(DepartmentRolesEditModel vm);
        Task<IQueryable<Department>> get_user_departments_async(string user_id, DepartmentAccessLevels level);
        IQueryable<Department> get_user_departments(string user_id, DepartmentAccessLevels level);
    }
    public class DepartmentAccessService : IDepartmentAccessService
    {
        CerberusDBEntities _db;
        IUserService _userService;
        IGroupService _groupService;
        public DepartmentAccessService(CerberusDBEntities db, IUserService userService, IGroupService roleService)
        {
            _db = db;
            _groupService = roleService;
            _userService = userService;
        }


        public async Task updatePreviligedGroups(DepartmentRolesEditModel vm)
        {
            _db.GroupDepartmentClaims.RemoveRange(_db.GroupDepartmentClaims.Where(p => p.department_id == vm.id));

            if (vm.Groups != null)
            {
                foreach (var r in vm.getGroupIDs())
                {
                    _db.GroupDepartmentClaims.Add(new GroupDepartmentClaim
                    {
                        department_id = vm.id,
                        group_id = (await _groupService.GetGroupByIdAsync(r)).Id,

                    });
                }
            }

            await _db.SaveChangesAsync();
        }


        public async Task<IQueryable<Department>> get_user_departments_async(string user_id, DepartmentAccessLevels level)
        {

            var groups = await _userService.GetUserGroupsByIdAsync(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);


            if (group_names.Contains("Admin"))
            {
                return _db.Departments;
            }

            switch (level)
            {
                case DepartmentAccessLevels.Full:
                    return _db.GroupDepartmentClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department);
                case DepartmentAccessLevels.Partial:
                    var deps = _db.GroupFactorySiteClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.FactorySite.Department)
                        .Union(_db.GroupWareHouseClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Warehouse.Department));
                    return _db.GroupDepartmentClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department).Union(deps);
                default:
                    throw new Exception();
            }


        }

        public IQueryable<Department> get_user_departments(string user_id, DepartmentAccessLevels level)
        {
            var groups = _userService.GetUserGroupsById(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);


            if (group_names.Contains("Admin"))
            {
                return _db.Departments;
            }

            switch (level)
            {
                case DepartmentAccessLevels.Full:
                    return _db.GroupDepartmentClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department);
                case DepartmentAccessLevels.Partial:
                    var deps = _db.GroupFactorySiteClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.FactorySite.Department)
                        .Union(_db.GroupWareHouseClaims
                            .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Warehouse.Department));
                    return _db.GroupDepartmentClaims
                        .Where(c => group_ids.Contains(c.group_id)).Select(c => c.Department).Union(deps);
                default:
                    throw new Exception();
            }
        }
    }
}