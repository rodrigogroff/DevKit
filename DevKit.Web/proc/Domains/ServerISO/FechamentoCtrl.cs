using LinqToDB;
using System.Linq;
using System.Web.Http;
using DataModel;
using System;
using System.IO;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class FechamentoServerISOController : ApiControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("api/FechamentoServerISO")]
        public IHttpActionResult Get()
        {
            using (var db = new AutorizadorCNDB())
            {
                string currentEmpresa = "";

                try
                {
                    var dt = DateTime.Now;

                    var diaFechamento = dt.Day;
                    var ano = dt.ToString("yyyy");
                    var mes = dt.ToString("MM").PadLeft(2, '0');

                    #region - fechamento - 

                    var lstEmpresas = db.T_Empresa.Where(y => y.nu_diaFech == diaFechamento).ToList();

                    foreach (var empresa in lstEmpresas)
                    {
                        // ------------------------------
                        // só fecha uma vez no mes
                        // ------------------------------

                        if (db.LOG_Fechamento.Any(y => y.st_ano == ano &&
                                                        y.st_mes == mes &&
                                                        y.fk_empresa == empresa.i_unique))
                            continue;

                        currentEmpresa = empresa.st_empresa;

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [INICIO]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes
                        });

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

                        long totValor = 0;

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
                                totValor += (int)parc.vr_valor;

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

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = null,
                            st_oper = "Fechamento [OK]",
                            st_empresa = currentEmpresa,
                            st_log = "Ano " + ano + " Mes " + mes + " Valor => " + totValor
                        });
                    }

                    #endregion

                    #region - confere geração auto de lotes -

                    var configPla = db.ConfigPlasticoEnvio.FirstOrDefault(y => y.id == 1);

                    if (configPla != null)
                    {
                        if (configPla.bAtivo == true)
                        {
                            if (configPla.dom == true && DateTime.Now.DayOfWeek == DayOfWeek.Sunday ||
                                configPla.seg == true && DateTime.Now.DayOfWeek == DayOfWeek.Monday ||
                                configPla.ter == true && DateTime.Now.DayOfWeek == DayOfWeek.Tuesday ||
                                configPla.qua == true && DateTime.Now.DayOfWeek == DayOfWeek.Wednesday ||
                                configPla.qui == true && DateTime.Now.DayOfWeek == DayOfWeek.Thursday ||
                                configPla.sex == true && DateTime.Now.DayOfWeek == DayOfWeek.Friday ||
                                configPla.sab == true && DateTime.Now.DayOfWeek == DayOfWeek.Saturday )
                            {
                                if (dt.Hour.ToString().PadLeft(2,'0') + dt.Minute.ToString().PadLeft (2,'0') == configPla.stHorario )
                                {
                                    var arquivo = "teste.txt";

                                    var myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img//" + arquivo;

                                    if (File.Exists(myPath))
                                        File.Delete(myPath);

                                    using (var sw = new StreamWriter(myPath, false, Encoding.UTF8))
                                    {
                                        sw.WriteLine("teste!");
                                    }

                                    new ApiControllerBase().SendEmail("Arquivo Plástico", "https://meuconvey.conveynet.com.br//img//"  + arquivo, configPla.stEmails);
                                }
                            }
                        }
                    }

                    #endregion
                }
                catch (SystemException ex)
                {
                    db.Insert(new LOG_Audit
                    {
                        dt_operacao = DateTime.Now,
                        fk_usuario = null,
                        st_oper = "Fechamento [ERRO]",
                        st_empresa = currentEmpresa,
                        st_log = ex.ToString()
                    });
                }
            }

            return Ok();
        }
    }
}
