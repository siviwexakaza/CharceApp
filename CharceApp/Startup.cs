using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CharceApp.Startup))]
namespace CharceApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
