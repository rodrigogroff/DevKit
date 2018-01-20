using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.Threading;
using DataModel;
using LinqToDB;
using System;

namespace DevKit.Web
{
	public class Application : HttpApplication
	{
		protected void Application_Start()
		{
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
            
           // while (true)
            {
                using (var db = new AutorizadorCNDB())
                {
                    var dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day );
                    var dt_fim = dt.AddDays(1);
                    
                    foreach (var log in db.LOG_Transacoes.
                                        Where(y => y.dt_transacao > dt && y.dt_transacao < dt_fim && y.tg_confirmada.ToString() == "0").
                                        ToList())
                    {
                        log.tg_confirmada = '1';
                        db.Update(log);
                    }

                    //Thread.Sleep(60000);
                }
            }
        }
	}
}
