using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace cerberus.Models
{
    public class WareHouseAuthorizeAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public string Parameter { get; set; } = "id";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            int warehouse_id;
            if (filterContext.RouteData.Values.ContainsKey(Parameter))
                warehouse_id = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            else
                warehouse_id = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Form[Parameter]);

            var cont = (Controller)filterContext.Controller;
            using (var context = new CerberusDBEntities())
            {
                var user_id = cont.User.Identity.GetUserId();
                var userManager = cont.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
                var group_ids = ( userManager.GetRoles(user_id)).Select(r => roleManager.FindByName(r)).ToList();



                var warehouse_list = GroupWareHouseClaim.get_group_warehouses(context, userManager, user_id);

                if (!warehouse_list.Any(e => e.id == warehouse_id))
                {
                    filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this warehouse");
                    return;
                }

            }
        }
    }
}