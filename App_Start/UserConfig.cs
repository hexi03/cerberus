using cerberus.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Owin;

namespace cerberus.App_Start
{
    public class UserConfig
    {
        public static void Configure(IAppBuilder app)
        {
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            ApplicationUser adminUser = userManager.FindByName("Admin");

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",

                };

                var result = userManager.Create(adminUser, "password");
                adminUser = userManager.FindByName("admin");
                if (result.Succeeded)
                {
                    userManager.AddToRole(adminUser.Id, "Admin");
                }
            }
        }
    }
}