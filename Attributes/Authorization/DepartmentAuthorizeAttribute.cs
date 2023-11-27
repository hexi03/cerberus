using cerberus.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace cerberus.Models
{
    public class DepartmentAuthorizeAttribute : System.Web.Mvc.ActionFilterAttribute
    {

        public DepartmentAuthorizeAttribute()
        {

        }
        public DepartmentAccessLevels level { get; set; }

        public string Parameter { get; set; } = "id";
        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var _userService = DependencyResolver.Current.GetService<IUserService>();
            var _departmentAccessService = DependencyResolver.Current.GetService<IDepartmentAccessService>();

            int department_id;
            if (filterContext.RouteData.Values.ContainsKey(Parameter))
                department_id = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            else
                department_id = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Form[Parameter]);
            var cont = (Controller)filterContext.Controller;

            var user_id = cont.User.Identity.GetUserId();

            var group_ids = (_userService.GetUserGroupsById(user_id)).Select(r => r.Id).ToList();

            var departments_list =  _departmentAccessService.get_user_departments(user_id, level);

            if (!departments_list.Any(e => e.id == department_id))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this department");
                return;
            }

        }
    }
}