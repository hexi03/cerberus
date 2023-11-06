using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
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

            int warehouse_id = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            var cont = (Controller)filterContext.Controller;
            using (var context = new CerberusDBEntities())
            {
                var user_id = cont.User.Identity.GetUserId();
                var userManager = cont.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                var group_ids = userManager.GetRoles(user_id);



                var warehouse_list = GroupWareHouseClaim.get_group_warehouses(context, group_ids);

                if (!warehouse_list.Any(e => e.id == warehouse_id))
                {
                    filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this warehouse");
                    return;
                }

            }
        }
    }
}