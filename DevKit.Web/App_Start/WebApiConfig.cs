using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace DevKit.Web
{
	public static class WebApiConfig
	{
		public static string ControllerOnly = "ApiControllerOnly";
		public static string ControllerAndId = "ApiControllerAndIntegerId";
		public static string ControllerAction = "ApiControllerAction";

		public static void Register(HttpConfiguration config)
		{
			config.Filters.Add(new ValidationActionFilter());

			config.Formatters.XmlFormatter.MediaTypeMappings.Clear();
			config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

			config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			config.Formatters.JsonFormatter.SerializerSettings.Re‌ferenceLoopHandling = ReferenceLoopHandling.Ignore;
			config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.None;

			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

			config.MapHttpAttributeRoutes();

			var routes = config.Routes;

			routes.MapHttpRoute(
				name: ControllerOnly,
				routeTemplate: "api/{controller}"
			);

			routes.MapHttpRoute(
				name: ControllerAndId,
				routeTemplate: "api/{controller}/{id}",
				defaults: null, 
				constraints: new { id = @"^\d+$" } 
			);

			routes.MapHttpRoute(
				name: ControllerAction,
				routeTemplate: "api/{controller}/{action}"
			);
		}
	}
}
