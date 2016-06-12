using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AmazonWebApp.Startup))]
namespace AmazonWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
