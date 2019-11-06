using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmployeeInformationSystem.WebUI.Startup))]
namespace EmployeeInformationSystem.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
