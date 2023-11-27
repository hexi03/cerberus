using cerberus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace cerberus.Services
{

    public interface IUserService
    {
        Task<IdentityResult> AddPasswordAsync(string id, string password);
        Task<IdentityResult> AddToRoleAsync(string user_id, string group_id);
        Task<IdentityResult> CreateAsync(ApplicationUser applicationUser, string password);
        Task<IdentityResult> DeleteAsync(string user_id);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<ApplicationUser> GetUserByNameAsync(string name);
        Task<IList<IdentityRole>> GetUserGroupsByIdAsync(string userId);
        IList<IdentityRole> GetUserGroupsById(string userId);
        Task<IdentityResult> RemoveFromRoleAsync(string id, IdentityRole role);
        Task<IdentityResult> RemovePasswordAsync(string id);
        Task<IdentityResult> UpdateAsync(ApplicationUser user_entity);



    }
    public class UserService : IUserService
    {
        private readonly ApplicationUserManager _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(ApplicationUserManager userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> AddPasswordAsync(string id, string password)
        {
            return await _userManager.AddPasswordAsync(id, password);
        }

        public async Task<IdentityResult> AddToRoleAsync(string user_id, string group_id)
        {
            return await _userManager.AddToRoleAsync(user_id, (await _roleManager.FindByIdAsync(group_id)).Name);
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser applicationUser, string password)
        {
            return await _userManager.CreateAsync(applicationUser, password);
        }

        public async Task<IdentityResult> DeleteAsync(string user_id)
        {

            return await _userManager.DeleteAsync(await _userManager.FindByIdAsync(user_id));
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string name)
        {
            return await _userManager.FindByNameAsync(name);
        }

        public IList<IdentityRole> GetUserGroupsById(string userId)
        {
            List<IdentityRole> roles = new List<IdentityRole>();

            var g_raw =  _userManager.GetRoles(userId);
            foreach (var r in (g_raw))
            {
                roles.Add( _roleManager.FindByName(r));
            }
            return roles;
        }

        public async Task<IList<IdentityRole>> GetUserGroupsByIdAsync(string userId)
        {
            List<IdentityRole> roles = new List<IdentityRole>();
            
            var g_raw = await _userManager.GetRolesAsync(userId);
            foreach (var r in (g_raw))
            {
                roles.Add(await _roleManager.FindByNameAsync(r));
            }
            return roles;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(string id, IdentityRole role)
        {
            return await _userManager.RemoveFromRoleAsync(id, role.Name);
        }

        public async Task<IdentityResult> RemovePasswordAsync(string id)
        {
            return await _userManager.RemovePasswordAsync(id);
        }

        public async Task<IdentityResult> UpdateAsync(ApplicationUser user_entity)
        {
            return await _userManager.UpdateAsync(user_entity);
        }
    }
}