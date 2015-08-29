using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CombatTrackerServer.Startup))]
namespace CombatTrackerServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
