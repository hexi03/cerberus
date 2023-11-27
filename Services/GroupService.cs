using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace cerberus.Services
{

    public interface IGroupService
    {
        Task<IdentityResult> CreateAsync(IdentityRole identityRole);
        Task<IdentityResult> DeleteAsync(IdentityRole identityRole);
        Task<List<IdentityRole>> GetAllGroupsAsync();

        Task<IdentityRole> GetGroupByIdAsync(string groupId);

        Task<IdentityRole> GetGroupByNameAsync(string name);
    }
    public class GroupService : IGroupService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GroupService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole identityRole)
        {
            return await _roleManager.CreateAsync(identityRole);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole identityRole)
        {
            return await _roleManager.DeleteAsync(identityRole);
        }

        public async Task<List<IdentityRole>> GetAllGroupsAsync()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetGroupByIdAsync(string groupId)
        {
            return await _roleManager.FindByIdAsync(groupId);
        }

        public async Task<IdentityRole> GetGroupByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }
    }
}