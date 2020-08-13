using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using DevKit.Web.Controllers;
using System.Net;

namespace DevKit.Web
{
	public class Application : HttpApplication
	{
		protected void Application_Start()
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

			//AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			//RouteTable.Routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			//Map attributes 
			//RouteTable.Routes.MapMvcAttributeRoutes();

			RouteTable.Routes.MapRoute(
				name: "Default",
				url: "{*url}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional  },
				namespaces: new[] { "DevKit.Web.Controllers" }
			);
        }

        public HttpApplicationState cache;

        public override void Init()
        {
          //  var x = new FechamentoServerISOController();
			//x.Get();
        }
    }
}
