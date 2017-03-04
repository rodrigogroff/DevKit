using System.Web.Optimization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace DevKit.Web
{
	public class Application : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			// Map attributes 
			RouteTable.Routes.MapMvcAttributeRoutes();

			RouteTable.Routes.MapRoute(
				name: "Default",
				url: "{*url}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional  },
				namespaces: new[] { "DevKit.Web.Controllers" }
			);
		}
	}
}
