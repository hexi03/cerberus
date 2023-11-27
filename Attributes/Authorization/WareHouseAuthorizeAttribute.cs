using cerberus.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace cerberus.Models
{
    public class WareHouseAuthorizeAttribute : ActionFilterAttribute
    {

        public WareHouseAuthorizeAttribute()
        {
        }

        public string Parameter { get; set; } = "id";

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _wareHouseAccessService = DependencyResolver.Current.GetService<IWareHouseAccessService>();

            int warehouseId;
            if (filterContext.RouteData.Values.ContainsKey(Parameter))
                warehouseId = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            else
                warehouseId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Form[Parameter]);

            var cont = (Controller)filterContext.Controller;
            var user_id = cont.User.Identity.GetUserId();
            var userManager = cont.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));

            var warehouseList = _wareHouseAccessService.get_user_warehouses(user_id);

            if (!warehouseList.Any(e => e.id == warehouseId))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this warehouse");
                return;
            }
        }
    }
}