using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403(Roles = "Admin")]
    public class GroupUserController : Controller
    {
        private CerberusDBEntities _db;
        private IUserService _userManager;
        private IGroupService _roleManager;
        private IMapper _mapper;

        public GroupUserController(
            CerberusDBEntities db,
            IUserService userManager,
            IGroupService roleManager,
            IMapper mapper
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: GroupUser
        public async Task<ActionResult> Index()
        {
            var userMapper = new ApplicationUserViewModel.Mapper();

            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            ViewBag.Users = (await _userManager.GetAllUsersAsync()).Select(r => userMapper.Convert((r, new List<IdentityRole>()), new ApplicationUserViewModel(), null)).ToList();
            return View();
        }

        // GET: GroupUser/Details/5
        public async Task<ActionResult> GroupDetails(string id)
        {
            var model = _mapper.Map<GroupViewModel>(await _roleManager.GetGroupByNameAsync(id));
            return View(model);
        }

        // GET: GroupUser/Create
        public ActionResult GroupCreate()
        {
            return View();
        }

        // POST: GroupUser/Create
        [HttpPost]
        public async Task<ActionResult> GroupCreate(string id)
        {
            try
            {
                if (id == null || id == "")
                {
                    return RedirectToAction("Index", "Home");
                }
                await _roleManager.CreateAsync(new IdentityRole(id));
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
            var model = _mapper.Map<GroupViewModel>(await _roleManager.GetGroupByNameAsync(id));

            return View(model);
        }

        // POST: GroupUser/Delete/5
        [HttpPost]
        public async Task<ActionResult> GroupDeleteConfirmed(string id)
        {
            await _roleManager.DeleteAsync(await _roleManager.GetGroupByNameAsync(id));

            return RedirectToAction("Index");

        }


        // GET: GroupUser/Details/5
        public async Task<ActionResult> UserDetails(string id)
        {
            
            var model = new ApplicationUserViewModel.Mapper().Convert(
                (await _userManager.GetUserByIdAsync(id), await _userManager.GetUserGroupsByIdAsync(id)),
                new ApplicationUserViewModel(),
                null
                );

            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            return View(model);
        }


        public async Task<ActionResult> UserCreate()
        {
            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            return View();
        }
        // POST: GroupUser/Create
        [HttpPost]
        public async Task<ActionResult> UserCreate(ApplicationUserCreateModel model)
        {

            if (ModelState.IsValid && model.Password != null && model.Password != "")
            {
                var res = await _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = model.UserName
                },
                    model.Password
                );
                if (!res.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                var newUser = await _userManager.GetUserByNameAsync(model.UserName);
                if (model.getGroupIDs() != null)
                {
                    foreach (var r in model.getGroupIDs())
                    {
                        await _userManager.AddToRoleAsync(newUser.Id, r);
                    }
                }

                return RedirectToAction("Index");
            }

            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            return View(model);



        }

        // GET: GroupUser/Edit/5
        public async Task<ActionResult> UserEdit(string id)
        {
            var model = new ApplicationUserEditModel.Mapper().Convert(
                (await _userManager.GetUserByIdAsync(id), await _userManager.GetUserGroupsByIdAsync(id)),
                new ApplicationUserEditModel(),
                null
                );

            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();

            
            return View(model);
        }

        // POST: GroupUser/Edit/5
        [HttpPost]
        public async Task<ActionResult> UserEdit(ApplicationUserEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user_entity = await _userManager.GetUserByIdAsync(model.Id);

                foreach (var role in await _userManager.GetUserGroupsByIdAsync(user_entity.Id))
                {
                    await _userManager.RemoveFromRoleAsync(user_entity.Id, role);
                }

                if (model.getGroupIDs() != null)
                {
                    foreach (var r in model.getGroupIDs())
                    {
                        await _userManager.AddToRoleAsync(user_entity.Id, r);
                    }
                }

                user_entity.UserName = model.UserName;
                await _userManager.UpdateAsync(user_entity);

                if (model.Password != null)
                {
                    await _userManager.RemovePasswordAsync(user_entity.Id);
                    await _userManager.AddPasswordAsync(user_entity.Id, model.Password);
                }

                return RedirectToAction("Index");

            }
            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            return View(model);

        }

        // GET: GroupUser/Delete/5
        public async Task<ActionResult> UserDelete(string id)
        {
            var model = new ApplicationUserViewModel.Mapper().Convert(
                (await _userManager.GetUserByIdAsync(id), await _userManager.GetUserGroupsByIdAsync(id)),
                new ApplicationUserViewModel(),
                null
                );


            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();


            return View(model);
        }

        // POST: GroupUser/Delete/5
        [HttpPost]
        public async Task<ActionResult> UserDeleteConfirmed(string id)
        {

            

            await _userManager.DeleteAsync(id);

            return RedirectToAction("Index");

        }
    }
}
