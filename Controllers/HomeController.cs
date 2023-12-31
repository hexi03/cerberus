﻿using cerberus.Models;
using System.Web.Mvc;

namespace cerberus.Controllers
{
    [ProvideMenu]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;

            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                /*
                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                var user = new ApplicationUser
                {
                    UserName = "admin", 
                    Email = "admin@example.com",
                };
                var result = userManager.Create(user, "password");
                */
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult About()
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.ReturnUrl = @Request.Url.PathAndQuery;
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}