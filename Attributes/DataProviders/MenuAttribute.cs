using cerberus.Models.edmx;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace cerberus.Models
{
    public class ProvideMenuAttribute : ActionFilterAttribute
    {

        public ProvideMenuAttribute()
        {

        }

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _db = DependencyResolver.Current.GetService<CerberusDBEntities>();
            var _userService = DependencyResolver.Current.GetService<IUserService>();
            var _groupService = DependencyResolver.Current.GetService<IGroupService>();
            var _departmentAccessService = DependencyResolver.Current.GetService<IDepartmentAccessService>();
            var _factorySiteAccessService = DependencyResolver.Current.GetService<IFactorySiteAccessService>();
            var _wareHouseAccessService = DependencyResolver.Current.GetService<IWareHouseAccessService>();

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }

            var cont = (Controller)filterContext.Controller;
            var user_id = cont.User.Identity.GetUserId();
            

            filterContext.Controller.ViewBag.UserID = user_id;

            filterContext.Controller.ViewBag.UserGroupIDs = _userService.GetUserGroupsById(user_id).Select(g => g.Id).ToList();
            filterContext.Controller.ViewBag.UserDepartments = _departmentAccessService.get_user_departments(user_id, DepartmentAccessLevels.Partial).AsNoTracking().ToList();
            filterContext.Controller.ViewBag.UserFactorySites = _factorySiteAccessService.get_user_factorysites(user_id).AsNoTracking().ToList();
            filterContext.Controller.ViewBag.UserWareHouses = _wareHouseAccessService.get_user_warehouses(user_id).AsNoTracking().ToList();
        }
    }
}