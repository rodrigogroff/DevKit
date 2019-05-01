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
using System.Data;

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

                new Thread(new ThreadStart(BatchService_ConfirmacaoAuto)).Start();
                new Thread(new ThreadStart(BatchService_Fechamento)).Start();
            }
        }

        protected void BatchService_Fechamento()
        {
            while (true)
            {
                try
                {
                    using (var db = new AutorizadorCNDB())
                    {
                        var dt = DateTime.Now;

                        var diaFechamento = dt.Day;
                        var horaAtual = dt.ToString("HHmm");
                        var ano = dt.ToString("yyyy");
                        var mes = dt.ToString("MM").PadLeft(2, '0');

                        var lstEmpresas = db.T_Empresa.Where(y => y.nu_diaFech == diaFechamento && y.st_horaFech == horaAtual).ToList();

                        foreach (var empresa in lstEmpresas)
                        {
                            // ------------------------------
                            // só fecha uma vez no mes
                            // ------------------------------

                            if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                            y.st_mes == mes &&
                                                            y.fk_empresa == empresa.i_unique))
                                continue;

                            var g_job = new T_JobFechamento
                            {
                                dt_inicio = DateTime.Now,
                                dt_fim = null,
                                fk_empresa = (int)empresa.i_unique,
                                st_ano = ano,
                                st_mes = mes
                            };

                            // ----------------------------
                            // registra job
                            // ----------------------------

                            g_job.i_unique = Convert.ToInt32(db.InsertWithIdentity(g_job));

                            // ----------------------------
                            // busca parcelas
                            // ----------------------------

                            foreach (var parc in db.T_Parcelas.Where(y => y.fk_empresa == empresa.i_unique && y.nu_parcela > 0).ToList())
                            {
                                // ----------------------------
                                // somente confirmadas
                                // ----------------------------

                                var logTrans = db.LOG_Transacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);

                                if (logTrans == null)
                                    continue;
                                else
                                {
                                    if (logTrans.tg_confirmada == null)
                                        continue;

                                    if (logTrans.tg_confirmada.ToString() != TipoConfirmacao.Confirmada)
                                        continue;
                                }                                   

                                // ----------------------------
                                // decrementa parcela
                                // ----------------------------

                                var parcUpd = db.T_Parcelas.FirstOrDefault(y => y.i_unique == parc.i_unique);
                                parcUpd.nu_parcela--;
                                db.Update(parcUpd);

                                // -------------------------------------------
                                // insere fechamento quando parcela zerar 
                                // -------------------------------------------

                                if (parcUpd.nu_parcela == 0)
                                {
                                    db.Insert(new LOG_Fechamento
                                    {
                                        dt_compra = logTrans.dt_transacao,
                                        dt_fechamento = DateTime.Now,
                                        fk_cartao = parc.fk_cartao,
                                        fk_empresa = parc.fk_empresa,
                                        fk_loja = parc.fk_loja,
                                        fk_parcela = (int)parc.i_unique,
                                        nu_parcela = parc.nu_parcela,
                                        st_afiliada = "",
                                        st_ano = ano,
                                        st_mes = mes,
                                        vr_valor = parc.vr_valor
                                    });
                                }
                            }

                            // ----------------------------
                            // registra job / finalizado!
                            // ----------------------------

                            g_job.dt_fim = DateTime.Now;

                            db.Update(g_job);
                        }
                    }                    
                }
                catch (SystemException ex)
                {

                }

                Thread.Sleep(1000 * 60);
            }
        }

        protected void BatchService_ConfirmacaoAuto()
        {
            while (true)
            {
                using (var db = new AutorizadorCNDB())
                {
                    var dtNow = DateTime.Now;

                    var dtIni = dtNow.AddSeconds(-60*6);
                    var dtFim = dtNow.AddDays(-2);

                    var queryX = db.LOG_Transacoes.
                                    Where(y => y.dt_transacao > dtFim && y.dt_transacao < dtIni && 
                                               y.tg_confirmada.ToString() == TipoConfirmacao.Pendente && 
                                               y.tg_contabil.ToString() == TipoCaptura.SITEF ).
                                    ToList();

                    db.Insert(new T_WebBlock
                    {
                        st_ip = "Count: " + queryX.Count() + " -> " + dtIni.ToString(),
                        fk_cartao = queryX.Count(),
                        dt_expire = DateTime.Now
                    });

                    foreach (var item in queryX)
                    {
                        item.tg_confirmada = Convert.ToChar(TipoConfirmacao.Confirmada);
                        item.st_msg_transacao = "Conf. Auto";

                        db.Update(item);
                    }
                }                    

                Thread.Sleep(5000);
            }
        }
    }
}
