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
                                                   new string[] {   "Código",
                                                                    "CNPJ",
                                                                    "Email",
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

                foreach (var it in lstLinks.Where (y=> y.fk_loja == (int) item.i_unique ).ToList())
                    empresas += lstEmpresas.
                                    FirstOrDefault(y => y.i_unique == it.fk_empresa).st_empresa.TrimStart('0') + ",";

                foreach (var it in lstTerms.Where(y => y.fk_loja == item.i_unique).ToList())
                    terminais += it.nu_terminal.ToString().TrimStart('0') + ",";

                myXLSWrapper.AddContents(new string[]
                {
                    item.st_loja,
                    item.nu_CNPJ,
                    item.st_email,
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
            var cnpj = Request.GetQueryStringValue("cnpj");
            var nomeSocial = Request.GetQueryStringValue("nomeSocial");
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

            if (!string.IsNullOrEmpty(cnpj))
            {
                query = from e in query
                        where e.nu_CNPJ == cnpj
                        select e;
            }

            if (!string.IsNullOrEmpty(nomeSocial))
            {
                query = from e in query
                        where e.st_social.ToUpper().Contains(nomeSocial.ToUpper())
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

            var lst = new List<T_Loja>();

            var lstEmps = db.T_Empresa.ToList();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var senha = "Com Senha";

                if (item.tg_portalComSenha == 0)
                    senha = "Sem Senha";

                string strTerms = "", strEmps = "";

                foreach (var itemT in db.T_Terminal.Where(y => y.fk_loja == item.i_unique).ToList())
                    strTerms += itemT.nu_terminal.ToString() + ", ";

                foreach (var itemT in db.LINK_LojaEmpresa.Where(y => y.fk_loja == item.i_unique).ToList())
                {
                    var tE = lstEmps.FirstOrDefault(y => y.i_unique == itemT.fk_empresa);
                    strEmps += tE.st_empresa + ", ";
                }                    

                lst.Add(new T_Loja
                {
                    id = item.i_unique.ToString(),
                    terminal = item.st_loja,
                    nu_CNPJ = item.nu_CNPJ,
                    nome = item.st_nome,
                    st_social = item.st_social,
                    cidade = item.st_cidade,
                    estado = item.st_estado,
                    situacao = item.tg_blocked == '1' ? "Bloqueada" : "Ativa",
                    tipoVenda = senha,
                    strTerminais = strTerms,
                    strEmpresas = strEmps
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

            var mdl = (from e in db.T_Loja where e.i_unique == id select e).FirstOrDefault();

            if (mdl == null)
                return StatusCode(HttpStatusCode.NotFound);

            if (mdl.tg_portalComSenha == null)
                mdl.tg_portalComSenha = 1;

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
                                    where e.fk_loja == id 
                                    orderby e.i_unique
                                    select e).
                                    ToList())
            {
                lstTerminais.Add(new LojaTerminal
                {
                    id = item.i_unique.ToString(),
                    codigo = item.nu_terminal,
                    texto = item.st_localizacao != null ? item.st_localizacao.Replace ("{SE$3}",",") : ""
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

            // campos transformados
            mdl.id = mdl.i_unique.ToString();
            mdl.isentoFat = mdl.tg_isentoFat == 0 ? false : true;

            if (mdl.nu_pctValor != null) mdl.snuPctValor = mon.setMoneyFormat((long)mdl.nu_pctValor);
            if (mdl.vr_mensalidade != null) mdl.svrMensalidade = mon.setMoneyFormat((long)mdl.vr_mensalidade);
            if (mdl.vr_minimo != null) mdl.svrMinimo = mon.setMoneyFormat((long)mdl.vr_minimo);
            if (mdl.vr_transacao != null) mdl.svrTransacao = mon.setMoneyFormat((long)mdl.vr_transacao);
            if (mdl.nu_franquia != null) mdl.snuFranquia = mdl.nu_franquia != null ? mdl.nu_franquia.ToString().PadLeft(6,'0') : "000000";

            mdl.lstMensagens = lstMensagens;
            mdl.lstTerminais = lstTerminais;
            mdl.lstConvenios = lstConvenios;

            return Ok(mdl);
        }

        [HttpPost]
        public IHttpActionResult Post(T_Loja mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new SyCrafEngine.money();

            var mdlNew = new T_Loja
            {
                st_nome = mdl.st_nome,
                st_cidade = mdl.st_cidade,
                st_estado = mdl.st_estado,                
            };

            // campos transformados

            if (mdl.isentoFat == true)
                mdlNew.tg_isentoFat = 1;
            else
                mdlNew.tg_isentoFat = 0;

            if (mdlNew.tg_blocked == null)
                mdlNew.tg_blocked = '0';

            if (mdl.nu_periodoFat != null) mdlNew.nu_periodoFat = Convert.ToInt32(mdl.nu_periodoFat);
            if (mdl.nu_diavenc != null) mdlNew.nu_diavenc = Convert.ToInt32(mdl.nu_diavenc);
            if (mdl.snuPctValor != null) mdlNew.nu_pctValor = Convert.ToInt32(mon.prepareNumber(mdl.snuPctValor));
            if (mdl.svrMensalidade != null) mdlNew.vr_mensalidade = Convert.ToInt32(mon.prepareNumber(mdl.svrMensalidade));
            if (mdl.svrMinimo != null) mdlNew.vr_minimo = Convert.ToInt32(mon.prepareNumber(mdl.svrMinimo));
            if (mdl.svrTransacao != null) mdlNew.vr_transacao = Convert.ToInt32(mon.prepareNumber(mdl.svrTransacao));
            if (mdl.snuFranquia != null) mdlNew.nu_franquia = Convert.ToInt32(mon.prepareNumber(mdl.snuFranquia));

            // ----------------
            // obter codigo
            // ----------------

            var cd = Convert.ToInt32(
                        db.T_Loja.
                            Where(y => Convert.ToInt64(y.st_loja) > 6300).
                            OrderByDescending(y => y.st_loja).                            
                            Select ( y=> y.st_loja).
                            FirstOrDefault()) + 1;

            mdlNew.st_loja = cd.ToString();

            mdlNew.i_unique = Convert.ToInt64(db.InsertWithIdentity(mdlNew));

            // -------------
            // terminal
            // -------------

            var ult = db.T_Terminal.
                            Where(y => Convert.ToInt32(y.nu_terminal) > 7700).
                            Where(y => Convert.ToInt32(y.nu_terminal) < 8000).
                            OrderByDescending(y => y.nu_terminal).FirstOrDefault();

            var novo = new T_Terminal
            {
                nu_terminal = (Convert.ToInt32(ult.nu_terminal) + 1).ToString(),
                fk_loja = (int)mdlNew.i_unique,
                st_localizacao = "Terminal Automático"
            };

            novo.i_unique = Convert.ToInt64(db.InsertWithIdentity(novo));

            return Ok(mdlNew);
        }

        [HttpPut]
        public IHttpActionResult Put(T_Loja mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var mon = new SyCrafEngine.money();

            var mdlUpdate = (from e in db.T_Loja
                             where e.i_unique == Convert.ToInt32(mdl.id)
                             select e).
                             FirstOrDefault();

            
            #region - terminal - 

            if (mdl.novoTerminal != null)
            {
                var ult = db.T_Terminal.
                            Where( y=> Convert.ToInt32(y.nu_terminal) > 7700).
                            Where(y => Convert.ToInt32(y.nu_terminal) < 8000).
                            OrderByDescending(y => y.nu_terminal).FirstOrDefault();

                var novo = new T_Terminal
                {
                    nu_terminal = (Convert.ToInt32(ult.nu_terminal) + 1).ToString(),
                    fk_loja = (int)mdl.i_unique,
                    st_localizacao = mdl.novoTerminal.texto
                };

                novo.i_unique = Convert.ToInt64(db.InsertWithIdentity(novo));

                return Ok(new LojaTerminal
                {
                    texto = novo.st_localizacao,
                    codigo = novo.nu_terminal,
                    id = novo.i_unique.ToString()
                });
            }

            if (mdl.editTerminal != null)
            {
                var ed = db.T_Terminal.FirstOrDefault(y => y.i_unique.ToString() == mdl.editTerminal.id);

                if (ed == null)
                    return BadRequest("Terminal não disponível");

                try
                {
                    ed.st_localizacao = mdl.editTerminal.texto;

                    db.Update(ed);

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }
            }

            #endregion
                        
            #region - convenio - 

            if (mdl.novoConvenio != null)
            {
                if (db.LINK_LojaEmpresa.Any ( y=> y.fk_empresa == Convert.ToInt32(mdl.novoConvenio.idEmpresa) && y.fk_loja == (int)mdl.i_unique))
                    return BadRequest("Empresa já conveniada!");

                var novo = new LINK_LojaEmpresa
                {
                    fk_empresa = Convert.ToInt32(mdl.novoConvenio.idEmpresa),
                    fk_loja = (int)mdl.i_unique,
                    tx_admin = (int)ObtemValor(mdl.novoConvenio.tx_admin)
                };

                novo.i_unique = Convert.ToInt64(db.InsertWithIdentity(novo));

                var tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique.ToString() == mdl.novoConvenio.idEmpresa);

                var resp = new LojaConvenio
                {
                    id = novo.i_unique.ToString(),
                    empresa = tEmp.st_empresa + " " + tEmp.st_fantasia,
                    tx_admin = mon.setMoneyFormat((long)novo.tx_admin)
                };

                return Ok(resp);
            }

            if (mdl.editConvenio != null)
            {
                var convUp = db.LINK_LojaEmpresa.
                                FirstOrDefault(y => y.i_unique.ToString() == mdl.editConvenio.id);

                if (convUp == null)
                    return BadRequest("Convênio não disponível");

                try
                {
                    convUp.tx_admin = (int)ObtemValor(mdl.editConvenio.tx_admin);

                    db.Update(convUp);

                    return Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.ToString());
                }                
            }

            #endregion

            mdlUpdate.st_nome = mdl.st_nome;
            mdlUpdate.st_cidade = mdl.st_cidade;
            mdlUpdate.st_estado = mdl.st_estado;
            mdlUpdate.tg_portalComSenha = Convert.ToInt32(mdl.tg_portalComSenha);

            // campos transformados

            if (mdl.isentoFat == true)
                mdlUpdate.tg_isentoFat = 1;
            else
                mdlUpdate.tg_isentoFat = 0;

            mdlUpdate.nu_periodoFat = Convert.ToInt32(mdl.nu_periodoFat);
            mdlUpdate.nu_diavenc = Convert.ToInt32(mdl.nu_diavenc);

            mdlUpdate.nu_pctValor = Convert.ToInt32(mon.prepareNumber(mdl.snuPctValor));
            mdlUpdate.vr_mensalidade = Convert.ToInt32(mon.prepareNumber(mdl.svrMensalidade));
            mdlUpdate.vr_minimo = Convert.ToInt32(mon.prepareNumber(mdl.svrMinimo));
            mdlUpdate.vr_transacao = Convert.ToInt32(mon.prepareNumber(mdl.svrTransacao));
            mdlUpdate.nu_franquia = Convert.ToInt32(mon.prepareNumber(mdl.snuFranquia));

            mdlUpdate.st_email = mdl.st_email;
            mdlUpdate.nu_fax = mdl.nu_fax;
            mdlUpdate.nu_telefone = mdl.nu_telefone;
            mdlUpdate.st_telCelular = mdl.st_telCelular;

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
