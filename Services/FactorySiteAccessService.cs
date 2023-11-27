using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace cerberus.Services
{
    public interface IFactorySiteAccessService
    {
        Task updatePreviligedGroups(FactorySiteRolesEditModel vm);
        Task<IQueryable<FactorySite>> get_user_factorysites_async(string user_id);
        IQueryable<FactorySite> get_user_factorysites(string user_id);
    }
    public class FactorySiteAccessService : IFactorySiteAccessService
    {
        CerberusDBEntities _db;
        IUserService _userService;
        IGroupService _groupService;
        IDepartmentAccessService _departmentAccessService;
        public FactorySiteAccessService(CerberusDBEntities db, IUserService userService, IGroupService roleService, IDepartmentAccessService departmentAccessService)
        {
            _db = db;
            _groupService = roleService;
            _userService = userService;
            _departmentAccessService = departmentAccessService;
        }
        public async Task updatePreviligedGroups(FactorySiteRolesEditModel vm)
        {
            _db.GroupFactorySiteClaims.RemoveRange(_db.GroupFactorySiteClaims.Where(p => p.factorysite_id == vm.id));

            if (vm.Roles != null)
            {
                foreach (var r in vm.getGroupIDs())
                {
                    _db.GroupFactorySiteClaims.Add(new GroupFactorySiteClaim
                    {
                        factorysite_id = vm.id,
                        group_id = (await _groupService.GetGroupByIdAsync(r)).Id
                    });
                }
            }

            await _db.SaveChangesAsync();
        }

        public async Task<IQueryable<FactorySite>> get_user_factorysites_async(string user_id)
        {
            var groups = await _userService.GetUserGroupsByIdAsync(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);

            if (group_names.Contains("Admin"))
            {
                return _db.FactorySites.Include(e => e.Department);
            }
            var deps = (await _departmentAccessService.get_user_departments_async(user_id, DepartmentAccessLevels.Full)).Select(e => e.id);
            var factorysites = _db.GroupFactorySiteClaims
                    .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.FactorySite.department_id)).Select(c => c.FactorySite).Include(e => e.Department);
            return factorysites;

        }

        public IQueryable<FactorySite> get_user_factorysites(string user_id)
        {
            var groups = _userService.GetUserGroupsById(user_id);
            var group_names = groups.Select(g => g.Name);
            var group_ids = groups.Select(g => g.Id);

            if (group_names.Contains("Admin"))
            {
                return _db.FactorySites.Include(e => e.Department);
            }
            var deps = ( _departmentAccessService.get_user_departments(user_id, DepartmentAccessLevels.Full)).Select(e => e.id);
            var factorysites = _db.GroupFactorySiteClaims
                    .Where(c => group_ids.Contains(c.group_id) || deps.Contains(c.FactorySite.department_id)).Select(c => c.FactorySite).Include(e => e.Department);
            return factorysites;
        }
    }
}