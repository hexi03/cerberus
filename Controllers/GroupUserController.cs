using cerberus.DTO;
using cerberus.Models;
using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403(Roles = "Admin")]
    public class GroupUserController : Controller
    {
        private CerberusDBEntities db = new CerberusDBEntities();
        private RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
        private ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));


        // GET: GroupUser
        public ActionResult Index()
        {
            ViewBag.Roles = roleManager.Roles.ToList();
            ViewBag.Users = userManager.Users.ToList();
            return View();
        }

        // GET: GroupUser/Details/5
        public async Task<ActionResult> GroupDetails(string id)
        {
            var group = await roleManager.FindByNameAsync(id);
            if (group == null) {
                return RedirectToAction("Index", "Home");
            }
            return View(group);
        }

        // GET: GroupUser/Create
        public ActionResult GroupCreate()
        {
            return View();
        }

        // POST: GroupUser/Create
        [HttpPost]
        public ActionResult GroupCreate(string id)
        {
            try
            {
                if (id == null || id == "") {
                    return RedirectToAction("Index", "Home");
                }
                roleManager.Create(new IdentityRole(id));
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        // GET: GroupUser/Delete/5
        public async Task<ActionResult> GroupDelete(string id)
        {
            var group = await roleManager.FindByNameAsync(id);
            if (group == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(group);
        }

        // POST: GroupUser/Delete/5
        [HttpPost]
        public async Task<ActionResult> GroupDeleteConfirmed(string id)
        {
            await roleManager.DeleteAsync(await roleManager.FindByNameAsync(id));    
            
            return RedirectToAction("Index");

        }


        // GET: GroupUser/Details/5
        public async Task<ActionResult> UserDetails(string id)
        {
            var user = await userManager.FindByNameAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Roles = roleManager.Roles.ToList();
            return View(user);
        }

        // GET: GroupUser/Create
        public ActionResult Create()
        {
            ViewBag.Roles = roleManager.Roles.ToList();
            return View();
        }

        public async Task<ActionResult> UserCreate()
        {
            ViewBag.Roles = roleManager.Roles.ToList();
            return View();
        }
            // POST: GroupUser/Create
        [HttpPost]
        public async Task<ActionResult> UserCreate(ApplicationUserDTO user)
        {
            try
            {
                if (ModelState.IsValid && user.Password != null && user.Password != "")
                {
                    var res = await userManager.CreateAsync(new ApplicationUser(){
                        UserName = user.Name
                        },
                        user.Password
                    );
                    if (!res.Succeeded) {
                        return RedirectToAction("Index", "Home");
                    }
                    var newUser = await userManager.FindByNameAsync(user.Name);
                    if (user.Roles != null) {
                        foreach(var r in user.Roles)
                        {
                            await userManager.AddToRoleAsync(newUser.Id, r.Value);
                        }
                    }

                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return View();
            }
        }

        // GET: GroupUser/Edit/5
        public async Task<ActionResult> UserEdit(string id)
        {
            var user = await userManager.FindByNameAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Roles = roleManager.Roles.ToList();
            return View(new ApplicationUserDTO (){
                Name = user.UserName,
                Roles = user.Roles.Select(r => roleManager.FindById(r.RoleId).Name).ToDictionary(kv => kv, kv => Guid.NewGuid().ToString())
            });
        }

        // POST: GroupUser/Edit/5
        [HttpPost]
        public async Task<ActionResult> UserEdit(ApplicationUserDTO user)
        {
            try
            {
                var user_entity = await userManager.FindByNameAsync(user.Name);
                if (user_entity == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                if (user.Roles != null)
                {
                    foreach (var role in await userManager.GetRolesAsync(user_entity.Id))
                    {
                        await userManager.RemoveFromRoleAsync(user_entity.Id, role);
                    }
                
                    foreach (var r in user.Roles)
                    {
                        await userManager.AddToRoleAsync(user_entity.Id, r.Value);
                    }
                }

                await userManager.UpdateAsync(user_entity);

                if (user.Password != null)
                {
                    await userManager.RemovePasswordAsync(user_entity.Id);
                    await userManager.AddPasswordAsync(user_entity.Id, user.Password);
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GroupUser/Delete/5
        public async Task<ActionResult> UserDelete(string id)
        {
            var user = await userManager.FindByNameAsync(id);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Roles = roleManager.Roles.ToList();
            return View(user);
        }

        // POST: GroupUser/Delete/5
        [HttpPost]
        public async Task<ActionResult> UserDeleteConfirmed(string id)
        {

                var user = await userManager.FindByNameAsync(id);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                await userManager.DeleteAsync(user);

                return RedirectToAction("Index");

        }
    }
}
