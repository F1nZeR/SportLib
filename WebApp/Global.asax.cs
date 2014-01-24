using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebApp.CustomViewEngines;
using WebApp.Data;

namespace WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            using (var context = new DataContext())
            {
                context.Database.Initialize(false);
            }
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterViewEngine(ViewEngines.Engines);

        }

        public static void RegisterViewEngine(ViewEngineCollection viewEngines)
        {
            viewEngines.Clear();

            var themeableRazorViewEngine = new ThemeableRazorViewEngine
            {
                CurrentTheme = httpContext => httpContext.Session["theme"] as string ?? string.Empty
            };

            viewEngines.Add(themeableRazorViewEngine);
        }
    }
}
