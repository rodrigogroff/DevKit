using LinqToDB;
using System.Linq;
using System.Web.Http;
using DataModel;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.Razor.Generator;
using DocumentFormat.OpenXml.Bibliography;

namespace DevKit.Web.Controllers
{
    public class FechamentoServerISOController : ApiControllerBase
    {
        public string calculaCodigoAcesso(string empresa, string matricula, string titularidade, string via, string cpf)
        {
            string chave = matricula + empresa + titularidade.PadLeft(2, '0') + via + cpf.PadRight(14, ' ');
            int temp = 0;
            for (int n = 0; n < chave.Length; n++)
            {
                string s = chave.Substring(n, 1);
                char c = s[0]; // First character in s
                int i = c; // ascii code
                temp += i;
            }
            string calculado = temp.ToString("0000");
            temp += int.Parse(calculado[3].ToString()) * 1000;
            temp += int.Parse(calculado[2].ToString());
            if (temp > 9999) temp -= 2000;
            calculado = temp.ToString("0000");
            calculado = calculado.Substring(2, 1) +
                        calculado.Substring(0, 1) +
                        calculado.Substring(3, 1) +
                        calculado.Substring(1, 1);
            return calculado;
        }

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
                                    var lstCarts = (from e in db.T_Cartao
                                                    where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido
                                                    where e.tg_status.ToString() == CartaoStatus.Habilitado
                                                    select (int)e.i_unique).
                                        ToList();

                                    var lstLotesAbertos_ = db.T_LoteCartao.Where(y => y.tg_sitLote == 1).ToList();

                                    var lstLotesAbertos = db.T_LoteCartao.Where(y => y.tg_sitLote == 1).ToList().Select( y=> (int)y.i_unique).ToList();

                                    var lstLoteDets = db.T_LoteCartaoDetalhe.
                                                            Where(y => lstLotesAbertos.Contains((int)y.fk_lote)).
                                                            Select(y => (int)y.fk_cartao).
                                                            ToList();

                                    var lstCartsDisp = new List<int>();

                                    for (int i = 0; i < lstCarts.Count(); i++)
                                        if (!lstLoteDets.Contains(lstCarts[i]))
                                            lstCartsDisp.Add(lstCarts[i]);

                                    var lst = (from e in db.T_Cartao
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

                                    var lstEmp = lst.Select(y => y.empresa).Distinct().ToList();
                                    var lstEmbDb = db.T_Empresa.Where(y => lstEmp.Contains(y.st_empresa)).ToList();

                                    var lstArquivos = new List<string>();

                                    foreach (var st_emp in lstEmp)
                                    {
                                        if (st_emp != "000002")
                                            continue;

                                        var tEmp = lstEmbDb.FirstOrDefault(y => y.st_empresa == st_emp);
                                        var cartList = lst.Where(y => y.empresa == st_emp);

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

                                        var nomeArq = tEmp.st_fantasia.Trim() + "_" + tEmp.st_empresa + "_" + DateTime.Now.Day.ToString().PadLeft(2, '0') + "_" +
                                            DateTime.Now.Month.ToString().PadLeft(2, '0') + "_" + DateTime.Now.Year.ToString() + "_PEDIDO_PRODUCAO.txt";

                                        var myPath = System.Web.Hosting.HostingEnvironment.MapPath("/") + "img\\" + nomeArq;

                                        lstArquivos.Add("https://meuconvey.conveynet.com.br/img/" + nomeArq);                                        

                                        if (File.Exists(myPath))
                                            File.Delete(myPath);

                                        using (var sw = new StreamWriter(myPath, false, Encoding.UTF8))
                                        {
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

                                                sw.WriteLine(line);
                                            }
                                        }
                                    }

                                    var textoEmail = "<p>Este é um arquivo gerado automaticamente - não responder.</p>" +
                                                     "<p>Arquivos para impressão de plástico:</p><p>";

                                    foreach (var item in lstArquivos)
                                        textoEmail += item + "\n";

                                    textoEmail += "</p><p>CONVEYNET - " + DateTime.Now.Year + "</p>";

                                    new ApiControllerBase().SendEmail("Produção de Cartões", textoEmail, configPla.stEmails);                                    
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
