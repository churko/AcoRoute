using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AcoRoute.Startup))]
namespace AcoRoute
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
