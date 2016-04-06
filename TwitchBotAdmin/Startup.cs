using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TwitchBotAdmin.Startup))]
namespace TwitchBotAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
