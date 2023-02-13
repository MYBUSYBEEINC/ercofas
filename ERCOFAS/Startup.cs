using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ERCOFAS.Startup))]
namespace ERCOFAS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
