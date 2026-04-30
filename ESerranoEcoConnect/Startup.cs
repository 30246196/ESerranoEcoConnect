using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ESerranoEcoConnect.Startup))]
namespace ESerranoEcoConnect
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
