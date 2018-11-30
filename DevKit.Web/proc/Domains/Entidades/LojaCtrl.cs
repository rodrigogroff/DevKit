using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System;
using DataModel;
using App.Web;

namespace DevKit.Web.Controllers
{
    public class LojaController : ApiControllerBase
    {
        #region - exportar -

        [HttpGet]
        [AllowAnonymous]
        [Route("api/loja/exportar", Name = "ExportarLojas")]
        public IHttpActionResult Exportar()
        {
            return Get(exportar: true);
        }

        [NonAction]
        private IHttpActionResult Export( IQueryable<T_Loja> query)
        {
            var myXLSWrapper = new ExportWrapper ( "Export_Lojas.xlsx",
                                                   "Lojas",
                                                   new string[] { "Código",
                                                                  "Nome",
                                                                  "Cidade",
                                                                  "Estado",
                                                                  "Endereço",
                                                                  "Telefone",
                                                                  "Empresas",
                                                                  "Terminais",
                                                                  "Bloqueio",
                                                                  "Senha" });

            var lstEmpresas = db.T_Empresa.ToList();
            var lstLinks = db.LINK_LojaEmpresa.ToList();
            var lstTerms = db.T_Terminal.ToList();

            foreach (var item in query)
            {
                string senha = "Com Senha", empresas = "", terminais = "";

                if (item.tg_portalComSenha == 0)
                    senha = "Sem Senha";

                foreach (var it in lstLinks.Where (y=> y.fk_loja == item.i_unique ).ToList())
                    empresas += lstEmpresas.
                                    FirstOrDefault(y => y.i_unique == it.fk_empresa).st_empresa.TrimStart('0') + ",";

                foreach (var it in lstTerms.Where(y => y.fk_loja == item.i_unique).ToList())
                    terminais += it.nu_terminal.ToString().TrimStart('0') + ",";

                myXLSWrapper.AddContents(new string[]
                {
                    item.st_loja,
                    item.st_nome,
                    item.st_cidade,
                    item.st_estado,
                    item.st_endereco,
                    item.nu_telefone.ToString(),
                    empresas,
                    terminais,
                    item.tg_blocked == '1' ? "Bloqueada" : "Ativa",
                    senha,
                });
            };

            return ResponseMessage(myXLSWrapper.GetSingleSheetHttpResponse());
        }

        #endregion

        public IHttpActionResult Get(bool? exportar = false)
        {
            var busca = Request.GetQueryStringValue("busca");
            var terminal = Request.GetQueryStringValue("terminal");
            var cidade = Request.GetQueryStringValue("cidade");
            var estado = Request.GetQueryStringValue("estado");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa");
            var bloqueada = Request.GetQueryStringValue<bool?>("bloqueada", null);
            var comSenha = Request.GetQueryStringValue<bool?>("comSenha", null);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Loja select e);

            if (!string.IsNullOrEmpty(busca))
            {
                query = from e in query
                        where e.st_nome.Contains(busca)
                        select e;
            }

            if (!string.IsNullOrEmpty(terminal))
            {
                query = from e in query
                        where e.st_loja == terminal
                        select e;
            }

            if (!string.IsNullOrEmpty(cidade))
            {
                query = from e in query
                        where e.st_cidade.ToUpper() == cidade
                        select e;
            }

            if (idEmpresa != null)
            {
                query = from e in query
                        join emp in db.LINK_LojaEmpresa on Convert.ToInt32(e.i_unique) equals emp.fk_loja
                        where emp.fk_empresa == idEmpresa
                        select e;
            }

            if (!string.IsNullOrEmpty(estado))
            {
                query = from e in query
                        where e.st_estado.ToUpper() == estado
                        select e;
            }

            if (bloqueada != null)
            {
                if (bloqueada == true)
                    query = from e in query
                            where e.tg_blocked.ToString() == "1"
                            select e;
                else
                    query = from e in query
                            where e.tg_blocked.ToString() == "0"
                            select e;
            }

            if (comSenha != null)
            {
                if (comSenha == true)
                    query = from e in query
                            where e.tg_portalComSenha == 1
                            select e;
                else
                    query = from e in query
                            where e.tg_portalComSenha != 1
                            select e;
            }

            query = query.OrderBy(y => y.st_nome);

            if (exportar == true)
                return Export(query);

            var lst = new List<Loja>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var senha = "Com Senha";

                if (item.tg_portalComSenha == 0)
                    senha = "Sem Senha";

                lst.Add(new Loja
                {
                    id = item.i_unique.ToString(),
                    terminal = item.st_loja,
                    nome = item.st_nome,
                    cidade = item.st_cidade,
                    estado = item.st_estado,
                    situacao = item.tg_blocked == '1' ? "Bloqueada" : "Ativa",
                    tipoVenda = senha,
                });
            }

            return Ok(new
            {
                count = query.Count(),
                results = lst
            });
        }
                
        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdl = (from e in db.T_Loja
                       where e.i_unique == id
                       select e).
                       FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            string comSenha = "1";

            if (mdl.tg_portalComSenha == 0)
                comSenha = "0";

            var lstMensagens = new List<LojaMensagem>();
            
            foreach (var item in (from e in db.T_LojaMensagem
                                  where e.fk_loja == id || e.fk_loja == 0
                                  orderby e.dt_validade
                                  select e).
                                  ToList() )
            {
                lstMensagens.Add(new LojaMensagem
                {
                    id = item.i_unique.ToString(),
                    link = item.st_link,
                    tipo = item.fk_loja > 0 ? "Mensagem privada" : "Institucional",
                    mensagem = item.st_msg,
                    validade = Convert.ToDateTime(item.dt_validade).ToString("dd/MM/yyyy"),
                    ativa = item.tg_ativa == null || item.tg_ativa == false ? false : true,
                    dia_final = item.dt_validade.Value.Day.ToString(),
                    mes_final = item.dt_validade.Value.Month.ToString(),
                    ano_final = item.dt_validade.Value.Year.ToString(),
                });
            }

            var lstTerminais = new List<LojaTerminal>();

            foreach (var item in (from e in db.T_Terminal
                                    where e.fk_loja == id || e.fk_loja == 0
                                    orderby e.i_unique
                                    select e).
                                    ToList())
            {
                lstTerminais.Add(new LojaTerminal
                {
                    id = item.i_unique.ToString(),
                    codigo = item.nu_terminal,
                    texto = item.st_localizacao.Replace ("{SE$3}",",")
                });
            }
            
            var lstConvenios = new List<LojaConvenio>();
            var mon = new SyCrafEngine.money();

            foreach (var item in (from e in db.LINK_LojaEmpresa
                                    where e.fk_loja == id || e.fk_loja == 0
                                    orderby e.i_unique
                                    select e).
                                    ToList())
            {
                var tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == item.fk_empresa);

                lstConvenios.Add(new LojaConvenio
                {
                    id = item.i_unique.ToString(),
                    empresa = tEmp.st_empresa + " " + tEmp.st_fantasia,
                    tx_admin = mon.setMoneyFormat((long)item.tx_admin)
                });
            }
            
            return Ok(new 
            {
                //campos diretos

                id = mdl.i_unique.ToString(),
                terminal = mdl.st_loja,
                nome = mdl.st_nome,
                cidade = mdl.st_cidade,
                estado = mdl.st_estado,
                tg_blocked = mdl.tg_blocked.ToString(),
                tg_portalComSenha = comSenha,
                mdl.nu_periodoFat,
                mdl.nu_diavenc,

                // campos transformados
                tg_isentoFat = mdl.tg_isentoFat == 0 ? false : true,
                snuPctValor = mon.setMoneyFormat((long)mdl.nu_pctValor),
                svrMensalidade = mon.setMoneyFormat((long)mdl.vr_mensalidade),
                svrMinimo = mon.setMoneyFormat((long)mdl.vr_minimo),
                svrTransacao = mon.setMoneyFormat((long)mdl.vr_transacao),
                snuFranquia = mon.setMoneyFormat((long)mdl.nu_franquia),

                // listas
                lstMensagens,
                lstTerminais,
                lstConvenios
            });
        }

        [HttpPut]
        public IHttpActionResult Put(Loja mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mdlUpdate = (from e in db.T_Loja
                             where e.i_unique == Convert.ToInt32(mdl.id)
                             select e).
                             FirstOrDefault();

            mdlUpdate.st_nome = mdl.nome;
            mdlUpdate.tg_portalComSenha = Convert.ToInt32(mdl.tg_portalComSenha);
            
            db.Update(mdlUpdate);

            if (mdl.novaMensagem != null)
            {
                if (mdl.novaMensagem.id == "" || mdl.novaMensagem.id == null)
                {
                    if (mdl.novaMensagem.mensagem != "" && mdl.novaMensagem.mensagem != null)
                    {
                        db.Insert(new T_LojaMensagem
                        {
                            dt_validade = new DateTime(Convert.ToInt32(mdl.novaMensagem.ano_final),
                                                    Convert.ToInt32(mdl.novaMensagem.mes_final),
                                                    Convert.ToInt32(mdl.novaMensagem.dia_final), 23, 59, 59),
                            fk_loja = Convert.ToInt64(mdl.id),
                            st_link = mdl.novaMensagem.link,
                            st_msg = mdl.novaMensagem.mensagem,
                            dt_criacao = DateTime.Now,
                            tg_ativa = true
                        });
                    }                    
                }
                else
                {
                    var dbItem = (from e in db.T_LojaMensagem
                                  where e.i_unique.ToString() == mdl.novaMensagem.id
                                  select e ).
                                  FirstOrDefault();

                    if (dbItem != null)
                    {
                        if (mdl.novaMensagem.mensagem != null)
                        {
                            dbItem.dt_validade = new DateTime(Convert.ToInt32(mdl.novaMensagem.ano_final),
                                                    Convert.ToInt32(mdl.novaMensagem.mes_final),
                                                    Convert.ToInt32(mdl.novaMensagem.dia_final), 23, 59, 59);

                            dbItem.st_link = mdl.novaMensagem.link;
                            dbItem.st_msg = mdl.novaMensagem.mensagem;
                            dbItem.tg_ativa = mdl.novaMensagem.ativa;

                            db.Update(dbItem);
                        }
                    }
                }
            }

            var lstTerminais = (from e in db.T_Terminal
                                where e.fk_loja.ToString() == mdl.id
                                select e).
                                ToList();

            foreach (var item in lstTerminais)
                CleanCache(db, CacheTags.T_Terminal, Convert.ToInt32(item.nu_terminal) );

            return Ok();
        }
    }
}
