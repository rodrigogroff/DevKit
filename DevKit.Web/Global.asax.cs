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
        }


        public HttpApplicationState cache;

        public override void Init()
        {
            cache = HttpContext.Current.Application;

            if (cache["started_observer"] == null)
            {
                cache["started_observer"] = true;
                new Thread(new ThreadStart(BatchService)).Start();
            }
        }

        protected void BatchService()
        {
            while (true)
            {
                using (var db = new AutorizadorCNDB())
                {
                    var dtNow = DateTime.Now;
                    var dtIni = dtNow.AddSeconds(-330);
                    var dtFim = dtNow.AddSeconds(-300);

                    var queryX = db.LOG_Transacoes.
                                    Where(y => y.dt_transacao > dtIni && y.dt_transacao < dtFim && 
                                               y.tg_confirmada.ToString() == TipoConfirmacao.Pendente && 
                                               y.tg_contabil.ToString() == TipoCaptura.SITEF ).
                                    ToList();

                    foreach (var item in queryX)
                    {
                        item.tg_confirmada = Convert.ToChar(TipoConfirmacao.Cancelada);
                        item.st_msg_transacao = "Canc. Auto";

                        db.Update(item);
                    }
                }                    

                Thread.Sleep(1000);
            }
        }
    }
}
