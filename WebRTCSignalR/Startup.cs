using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebRTCSignalR.Startup))]
namespace WebRTCSignalR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
