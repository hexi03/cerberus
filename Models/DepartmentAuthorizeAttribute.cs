using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace cerberus.Models
{
    public class DepartmentAuthorizeAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public GroupDepartmentClaim.Levels level { get; set; }

        public string Parameter { get; set; } = "id";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            int department_id;
            if (filterContext.RouteData.Values.ContainsKey(Parameter))
                department_id = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            else
                department_id = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Form[Parameter]);
            var cont = (Controller)filterContext.Controller;
            using (var context = new CerberusDBEntities())
            {
                var user_id = cont.User.Identity.GetUserId();
                var userManager = cont.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var group_ids = userManager.GetRoles(user_id);

                var departments_list = GroupDepartmentClaim.get_group_departments(context, group_ids, level);

                if (!departments_list.Any(e => e.id == department_id))
                {
                    filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this department");
                    return;
                }
            }
        }
    }
}