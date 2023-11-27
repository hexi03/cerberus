using cerberus.App_Start;
using cerberus.Mappers;
using cerberus.Models.edmx;
using cerberus.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(cerberus.Startup))]
namespace cerberus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            RoleConfig.Configure(app);
            UserConfig.Configure(app);

            

        }

    }
}
