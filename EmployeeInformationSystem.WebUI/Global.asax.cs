using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EmployeeInformationSystem.WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Start log4net
            log4net.Config.XmlConfigurator.Configure();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            CultureInfo cInfo = new CultureInfo("en-IN");
            cInfo.DateTimeFormat.ShortDatePattern = "dd-MM-yyyy";
            cInfo.DateTimeFormat.DateSeparator = "-";
            Thread.CurrentThread.CurrentCulture = cInfo;
            Thread.CurrentThread.CurrentUICulture = cInfo;
        }
    }
}
