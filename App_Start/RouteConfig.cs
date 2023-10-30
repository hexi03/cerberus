using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace cerberus
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "Default1",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            routes.MapRoute(
                name: "Home",
                url: "Home/{action}",
                defaults: new { controller = "Home" }
            );

            routes.MapRoute(
                name: "Reports",
                url: "Reports/{action}/{id}",
                defaults: new { controller = "Reports", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Departments",
                url: "Departments/{action}/{id}",
                defaults: new { controller = "Departments", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Warehouses",
                url: "Warehouses/{action}/{id}",
                defaults: new { controller = "Warehouses", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "FactorySites",
                url: "FactorySites/{action}/{id}",
                defaults: new { controller = "FactorySites", id = UrlParameter.Optional }
            );


        }
    }
}
