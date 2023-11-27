using AutoMapper;
using cerberus.Models;
using cerberus.Models.edmx;
using cerberus.Models.ViewModels;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    [Authorize403]
    public class DepartmentsController : Controller
    {
        private CerberusDBEntities _db;
        private IUserService _userManager;
        private IGroupService _roleManager;
        private IMapper _mapper;
        private IDepartmentAccessService _departmentAccessService;

        public DepartmentsController(
            CerberusDBEntities db,
            IUserService userManager,
            IGroupService roleManager,
            IMapper mapper,
            IDepartmentAccessService departmentAccessService
            )
        {
            _mapper = mapper;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _departmentAccessService = departmentAccessService;
        }

        public async Task<ActionResult> Index()
        {
            var user_id = User.Identity.GetUserId();

            var group_ids = _userManager.GetUserGroupsByIdAsync(user_id);
            List<DepartmentViewModel> model =
                (await (await _departmentAccessService.get_user_departments_async(user_id, DepartmentAccessLevels.Full)).ToListAsync())
                .Select(d => _mapper.Map<DepartmentViewModel>(d))
                .ToList();
            return View(model);
        }

        // GET: Departments/Details/5
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Details(int id)
        {
            var user_id = User.Identity.GetUserId();

            var group_ids = (await _userManager.GetUserGroupsByIdAsync(user_id)).Select(r => r.Id).ToList();

            DepartmentViewModel model = _mapper.Map<DepartmentViewModel>(await _db.Departments.FindAsync(id));

            return View(model);
        }

        // GET: Departments/Create
        [HttpGet]
        [Authorize403(Roles = "Admin")]

        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize403(Roles = "Admin")]

        public async Task<ActionResult> Create(DepartmentCreateModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Departments.Add(_mapper.Map<Department>(model));

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // GET: Departments/Edit/5
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Edit(int? id)
        {

            DepartmentEditModel model = _mapper.Map<DepartmentEditModel>(await _db.Departments.FindAsync(id));

            return View(model);
        }

        // POST: Departments/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Edit(DepartmentEditModel model)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(_mapper.Map<Department>(model)).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Departments/Delete/5
        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> Delete(int? id)
        {

            DepartmentViewModel model = _mapper.Map<DepartmentViewModel>(await _db.Departments.FindAsync(id));

            return View(model);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Department department = await _db.Departments.FindAsync(id);
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        public async Task<ActionResult> ManageAccess(int id)
        {
            var department = await _db.Departments.FindAsync(id);


            ViewBag.Groups = (await _roleManager.GetAllGroupsAsync()).Select(r => _mapper.Map<GroupViewModel>(r)).ToList();
            DepartmentRolesEditModel model = _mapper.Map<DepartmentRolesEditModel>(department);

            return View(model);
        }

        [Authorize403(Roles = "Admin")]
        [DepartmentAuthorize(level = DepartmentAccessLevels.Full)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> ManageAccess(DepartmentRolesEditModel model)
        {
            if (ModelState.IsValid)
            {
                await _departmentAccessService.updatePreviligedGroups(model);

                return RedirectToAction("Index");
            }
            return View(model);

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
