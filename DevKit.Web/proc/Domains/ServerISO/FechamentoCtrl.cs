using LinqToDB;
using System.Linq;
using System.Web.Http;
using DataModel;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

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

                var configPla = db.ConfigPlasticoEnvio.FirstOrDefault(y => y.id == 1);
                
                bool homolog = false;

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
                                    configPla.stStatus = "1";
                                    db.Update(configPla);

                                    List<string> lstAttach = new List<string>();
                                    List<string> lstLotesAbertos_lst = new List<string>();

                                    var lstCarts = (from e in db.T_Cartao
                                                    where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
                                                    where e.tg_status.ToString() == CartaoStatus.Habilitado
                                                    select (int)e.i_unique).
                                        ToList();

                                    var lstLotesAbertos_ = db.T_LoteCartao.Where(y => y.tg_sitLote == 1).ToList();

                                    foreach (var item in lstLotesAbertos_)
                                        lstLotesAbertos_lst.Add(item.i_unique.ToString());

                                    var lstLoteDets = db.T_LoteCartaoDetalhe.
                                                            Where(y => lstLotesAbertos_lst.Contains(y.fk_lote.ToString())).
                                                            Select(y => (int)y.fk_cartao).
                                                            ToList();

                                    var lstCartsDisp = new List<int>();

                                    for (int i = 0; i < lstCarts.Count(); i++)
                                        if (!lstLoteDets.Contains(lstCarts[i]))
                                            lstCartsDisp.Add(lstCarts[i]);

                                    configPla.stStatus = "2";
                                    db.Update(configPla);

                                    var lstMain = (from e in db.T_Cartao
                                               join d in db.T_Proprietario on e.fk_dadosProprietario equals (int)d.i_unique
                                               where lstCartsDisp.Contains((int)e.i_unique)
                                               select new
                                               {
                                                   id = e.i_unique.ToString(),
                                                   empresa = e.st_empresa,
                                                   matricula = e.st_matricula,
                                                   associado = d.st_nome,
                                                   cpf = d.st_cpf,
                                                   titularidade = e.st_titularidade,
                                                   via = e.nu_viaCartao,
                                                   venc = e.st_venctoCartao,
                                                   fkProp = e.fk_dadosProprietario
                                               }).
                                               ToList();

                                    configPla.stStatus = "3";
                                    db.Update(configPla);

                                    var lstEmp = lstMain.Select(y => y.empresa).Distinct().ToList();

                                    var lstArquivos = new List<string>();
                                    
                                    foreach (var st_emp in lstEmp)
                                    {
                                        if (homolog)
                                            if (st_emp != "000002")
                                                continue;

                                        var tEmp = db.T_Empresa.FirstOrDefault(y => y.st_empresa == st_emp);
                                        var cartList = lstMain.Where(y => y.empresa == st_emp).ToList();

                                        // ----------------
                                        // cria novo lote
                                        // ----------------

                                        var novoLote = new T_LoteCartao
                                        {
                                            dt_abertura = DateTime.Now,
                                            fk_empresa = (int)tEmp.i_unique,
                                            nu_cartoes = cartList.Count(),
                                            tg_sitLote = 2,
                                            dt_envio_grafica = DateTime.Now,
                                        };

                                        novoLote.i_unique = Convert.ToDecimal(db.InsertWithIdentity(novoLote));

                                        // ------------------
                                        // cria os detalhes
                                        // ------------------

                                        foreach (var item in cartList)
                                        {
                                            db.Insert(new T_LoteCartaoDetalhe
                                            {
                                                fk_lote = Convert.ToInt32(novoLote.i_unique),
                                                fk_cartao = Convert.ToInt32(item.id),
                                                fk_empresa = (int)tEmp.i_unique,
                                                nu_cpf = item.cpf,
                                                nu_matricula = Convert.ToInt32(item.matricula),
                                                nu_titularidade = Convert.ToInt32(item.titularidade),
                                                nu_via_original = item.via,
                                                st_nome_cartao = item.associado,
                                                dt_pedido = DateTime.Now,                                               
                                            });
                                        }

                                        // ---------------------
                                        // exporta em arquivo no servidor
                                        // ---------------------

                                        var tituloArq = tEmp.st_empresa + "_" + novoLote.i_unique + "_PEDIDO_PRODUCAO.txt";

                                        var myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + tituloArq;

                                        lstAttach.Add(myPath);

                                        lstArquivos.Add("https://meuconvey.conveynet.com.br/img/" + tituloArq );

                                        if (File.Exists(myPath))
                                            File.Delete(myPath);

                                        try
                                        {
                                            string total_file = "";

                                            var nome = "";

                                            foreach (var cart in cartList)
                                            {
                                                var line = "";

                                                if (cart.titularidade == "01")
                                                {
                                                    nome = db.T_Proprietario.FirstOrDefault(y => y.i_unique == cart.fkProp).st_nome;
                                                }
                                                else
                                                {
                                                    nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == cart.fkProp &&
                                                                                                y.nu_titularidade == Convert.ToInt32(cart.titularidade)).st_nome;
                                                }

                                                line += nome.PadRight(30, ' ').Substring(0, 30).TrimEnd(' ') + ",";
                                                line += cart.empresa + ",";
                                                line += cart.matricula.ToString().PadLeft(6, '0') + ",";

                                                line += cart.venc.Substring(0, 2) + "/" +
                                                        cart.venc.Substring(2, 2) + ",";

                                                line += calculaCodigoAcesso(cart.empresa,
                                                                                cart.matricula,
                                                                                cart.titularidade,
                                                                                cart.via.ToString(),
                                                                                cart.cpf) + ",";


                                                line += nome + ",|";

                                                line += "826766" + cart.empresa +
                                                                        cart.matricula +
                                                                        cart.titularidade +
                                                                        cart.via.ToString() +
                                                                "65" + cart.venc;

                                                line += "|";

                                                var c_update = db.T_Cartao.FirstOrDefault(a => a.i_unique.ToString() == cart.id);

                                                if (c_update.tg_emitido == Convert.ToInt32(StatusExpedicao.NaoExpedido))
                                                {
                                                    c_update.tg_emitido = Convert.ToInt32(StatusExpedicao.EmExpedicao);

                                                    db.Update(c_update);
                                                }                                                

                                                total_file += line;
                                            }

                                            File.WriteAllText(myPath, total_file);
                                            
                                        }
                                        catch (Exception ex1)
                                        {
                                            configPla.stStatus = "3.x " + ex1.ToString();
                                            db.Update(configPla);
                                            return Ok();
                                        }                                        
                                    }

                                    configPla.stStatus = "4";
                                    db.Update(configPla);

                                    var textoEmail = "<p>Este é um arquivo gerado automaticamente - não responder.</p>" +
                                                     "<p>Arquivos para impressão de plástico:</p><p>";

                                    foreach (var item in lstArquivos)
                                        textoEmail += "<a href='" + item + "' download target='_blank'>" + item + "</a>\n";

                                    {
                                        var myRelatName = "relat_envio_" + 
                                                            DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" +
                                                            DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + 
                                                            DateTime.Now.Year.ToString() + ".txt";
                                                                                
                                        var myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + myRelatName;

                                        if (File.Exists(myPath))
                                            File.Delete(myPath);

                                        lstAttach.Add(myPath);

                                        using (var sw = new StreamWriter(myPath, false, Encoding.UTF8))
                                        {
                                            sw.WriteLine("CONVEY BENEFICIOS");
                                            sw.WriteLine("RELATORIO DE ENVIO AUTOMATICO DE CARTÕES");
                                            sw.WriteLine("DATA:" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Month.ToString().PadLeft(2, '0') + "/" + DateTime.Now.Year.ToString() + " " + DateTime.Now.Hour.ToString().PadLeft(2,'0') + ":" + DateTime.Now.Minute.ToString().PadLeft(2, '0'));
                                            sw.WriteLine("");

                                            int iNumber = 1, tot_carts = 0;

                                            foreach (var st_emp in lstEmp)
                                            {
                                                if (homolog)
                                                    if (st_emp != "000002")
                                                        continue;

                                                var tEmp = db.T_Empresa.FirstOrDefault(y => y.st_empresa == st_emp);
                                                var cartList = lstMain.Where(y => y.empresa == st_emp);

                                                sw.WriteLine("Empresa: " + st_emp + " - " + tEmp.st_fantasia );

                                                foreach (var c in cartList)
                                                {
                                                    sw.WriteLine(iNumber++ + ") " + c.associado + " - " + st_emp + "." + c.matricula);
                                                    tot_carts++;
                                                }

                                                sw.WriteLine("Cartões na empresa: " + cartList.Count());
                                                sw.WriteLine("");
                                            }

                                            sw.WriteLine("");
                                            sw.WriteLine("Total de cartões enviados: " + tot_carts);
                                            sw.WriteLine("");
                                        }

                                        textoEmail += "</p><p><a href='https://meuconvey.conveynet.com.br/img/" + myRelatName + "' download target='_blank'>https://meuconvey.conveynet.com.br/img/" + myRelatName + "</a></p>";
                                    }

                                    textoEmail += "<p>&nbsp;</p><p>&nbsp;</p><p>CONVEYNET - " + DateTime.Now.Year + "</p>";

                                    configPla.stStatus = "5";
                                    db.Update(configPla);

                                    var emails = configPla.stEmails;

                                    //emails = "rodrigo.groff@gmail.com";

                                    if (SendEmail("Produção de Cartões", textoEmail, emails, lstAttach ))
                                    {
                                        configPla.stStatus = "6";
                                        db.Update(configPla);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }
                catch (Exception ex)                
                {
                    configPla.stStatus = ex.ToString();
                    db.Update(configPla);

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
