using cerberus.Models.edmx;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
                RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
                filterContext.Controller.ViewBag.UserID = user_id;

                var group_ids = userManager.GetRoles(user_id).Select(r => roleManager.FindByName(r)).ToList();
                filterContext.Controller.ViewBag.UserGroupIDs = group_ids;
                filterContext.Controller.ViewBag.UserDepartments = GroupDepartmentClaim.get_group_departments(context, userManager, user_id, GroupDepartmentClaim.Levels.Partial).AsNoTracking().ToList();
                filterContext.Controller.ViewBag.UserFactorySites = GroupFactorySiteClaim.get_group_factorysites(context, userManager, user_id).AsNoTracking().ToList();
                filterContext.Controller.ViewBag.UserWareHouses = GroupWareHouseClaim.get_group_warehouses(context, userManager, user_id).AsNoTracking().ToList();
            }
        }
    }
}