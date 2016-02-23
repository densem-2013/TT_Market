using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TT_Market.Web.Startup))]
namespace TT_Market.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
