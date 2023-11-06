using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace cerberus.Models
{
    public class ProvideMenuAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }
            var cont = (Controller)filterContext.Controller;
            using (var context = new CerberusDBEntities())
            {
                var user_id = cont.User.Identity.GetUserId();
                var userManager = cont.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                
                filterContext.Controller.ViewBag.UserID = user_id;

                var group_ids = userManager.GetRoles(user_id);
                filterContext.Controller.ViewBag.UserGroupIDs = group_ids;
                filterContext.Controller.ViewBag.UserDepartments = GroupDepartmentClaim.get_group_departments(context,group_ids, GroupDepartmentClaim.Levels.Partial).AsNoTracking().ToList();
                filterContext.Controller.ViewBag.UserFactorySites = GroupFactorySiteClaim.get_group_factorysites(context,group_ids).AsNoTracking().ToList();
                filterContext.Controller.ViewBag.UserWareHouses = GroupWareHouseClaim.get_group_warehouses(context,group_ids).AsNoTracking().ToList();
            }
        }
    }
}