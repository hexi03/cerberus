using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cerberus.Models;

namespace cerberus.App_Start
{
    public class RoleConfig
    {
        public static void Configure(IAppBuilder app) {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(ApplicationDbContext.Create()));
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
        }
    }
}