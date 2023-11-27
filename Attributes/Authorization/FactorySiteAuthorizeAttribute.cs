using cerberus.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;


namespace cerberus.Models
{
    public class FactorySiteAuthorizeAttribute : ActionFilterAttribute
    {


        public FactorySiteAuthorizeAttribute()
        {

        }

        public string Parameter { get; set; } = "id";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var _factorySiteAccessService = DependencyResolver.Current.GetService<IFactorySiteAccessService>();

            int factorySiteId;
            if (filterContext.RouteData.Values.ContainsKey(Parameter))
                factorySiteId = Convert.ToInt32(filterContext.RouteData.Values[Parameter]);
            else
                factorySiteId = Convert.ToInt32(filterContext.RequestContext.HttpContext.Request.Form[Parameter]);

            var cont = (Controller)filterContext.Controller;
            var user_id = cont.User.Identity.GetUserId();

            var factorySiteList = _factorySiteAccessService.get_user_factorysites(user_id);

            if (!factorySiteList.Any(e => e.id == factorySiteId))
            {
                filterContext.Result = new HttpStatusCodeResult(403, "You have not access to this factory site");
                return;
            }
        }
    }
}

