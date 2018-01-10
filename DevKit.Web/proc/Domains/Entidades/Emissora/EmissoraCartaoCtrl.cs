using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;

using SyCrafEngine;
using LinqToDB;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class CartaoListagemDTO
    {
        public string   id,
                        associado, 
                        situacao,
                        expedicao,
                        matricula,
                        dtInicial,
                        dtUltExp,
                        cartao,
                        cartaoTitVia,
                        cpf, 
                        tit, 
                        via,
                        dispM, 
                        limM, 
                        dispT, 
                        limT,
                        colorBack, colorFront,
                        limCota;

        public bool selecionado;
    }

    public class CartaoDependenteDTO
    {
        public string nome, tit, dt;
    }
    
    public class CartaoDTO
    {
        public string   id,
                        matricula,
                        nome,
                        cpf,
                        vencMes, vencAno,
                        dtNasc,
                        limMes, limTot, limCota,
                        banco, bancoAg, bancoCta,
                        tel,
                        email,
                        situacao, expedicao,
                        via,
                        uf, cidade, cep, end, numero, bairro,
                        modo, valor, array;

        public List<CartaoDependenteDTO> lstDeps = new List<CartaoDependenteDTO>();
    }

    public class EmissoraCartaoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var nome = Request.GetQueryStringValue("nome");
            var cpf = Request.GetQueryStringValue("cpf");
            var skip = Request.GetQueryStringValue<int>("skip", 0);
            var take = Request.GetQueryStringValue<int>("take", 1);
            var matricula = Request.GetQueryStringValue("matricula");

            var listagemCota = Request.GetQueryStringValue<bool>("cota", false);

            var idSit = Request.GetQueryStringValue<int?>("idSit", null);
            var idExp = Request.GetQueryStringValue<int?>("idExp", null);
            var idOrdem = Request.GetQueryStringValue<long?>("idOrdem", null);

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Cartao
                         where e.st_empresa == userLoggedEmpresa
                         select e);

            #region - filtros - 

            if (!String.IsNullOrEmpty(nome))
            {
                query = (from e in query
                         join p in db.T_Proprietario on e.fk_dadosProprietario equals (int)p.i_unique
                         where p.st_nome.ToUpper().Contains(nome)
                         select e);
            }

            if (!String.IsNullOrEmpty(cpf))
            {
                query = (from e in query
                         join p in db.T_Proprietario on e.fk_dadosProprietario equals (int)p.i_unique
                         where p.st_cpf == cpf
                         select e);
            }

            if (idSit != null)
            {
                idSit--; // zero não pode em combos

                query = (from e in query
                         where e.tg_status.ToString() == idSit.ToString()
                         select e);
            }

            if (idExp != null)
            {
                idExp--; // zero não pode em combos

                query = (from e in query
                         where e.tg_emitido.ToString() == idExp.ToString()
                         select e);
            }

            if (matricula != null && matricula != "")
            {
                query = (from e in query
                         where e.st_matricula == matricula.PadLeft(6, '0')
                         select e);
            }

            #endregion

            switch (idOrdem)
            {
                case null:
                case EnumOrdemEmissorManutCartoes.nomeAssociado:

                    query = (from e in query
                             join p in db.T_Proprietario on e.fk_dadosProprietario equals (int)p.i_unique
                             orderby p.st_nome, e.st_titularidade
                             select e);

                    break;

                case EnumOrdemEmissorManutCartoes.matricula:

                    query = (from e in query
                             orderby e.st_matricula, e.st_titularidade
                             select e);

                    break;
            }

            var res = new List<CartaoListagemDTO>();

            var mon = new money();
            var se = new StatusExpedicao();
            var sd = new SaldoDisponivel();
            var cs = new CartaoStatus();

            var lst = query.Skip(skip).Take(take).ToList();

            var lstIdAssoc = lst.Select(y => y.fk_dadosProprietario).ToList();

            var lstAssoc = (from e in db.T_Proprietario
                            where lstIdAssoc.Contains((int)e.i_unique)
                            select e).
                            ToList();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in lstAssoc
                             where e.i_unique == item.fk_dadosProprietario
                             select e).
                             FirstOrDefault();
                
                if (assoc != null)
                {
                    long dispM = 0, dispT = 0;

                    if (!listagemCota)
                        sd.Obter(db, item, ref dispM, ref dispT);

                    if (item.vr_extraCota == null) item.vr_extraCota = 0;
                    if (item.vr_limiteMensal == null) item.vr_limiteMensal = 0;
                    if (item.vr_limiteTotal == null) item.vr_limiteTotal = 0;

                    string colorBack = "", colorFront = "";

                    if (item.tg_status.ToString() == CartaoStatus.Bloqueado)
                    {
                        colorBack = "white";
                        colorFront = "black";
                    }

                    DateTime? dtPrimTtrans = null;

                    if (!listagemCota)
                        dtPrimTtrans = (from e in db.LOG_Transacoes
                                        where e.fk_cartao == item.i_unique
                                        select e.dt_transacao).
                                        FirstOrDefault();

                    if (!listagemCota)
                    {
                        res.Add(new CartaoListagemDTO
                        {
                            colorBack = colorBack,
                            colorFront = colorFront,
                            id = item.i_unique.ToString(),
                            via = item.nu_viaCartao.ToString(),
                            associado = assoc.st_nome,
                            cartaoTitVia = item.st_matricula + "." +
                                            item.st_titularidade + ":" +
                                            item.nu_viaCartao,
                            cpf = assoc.st_cpf,
                            situacao = cs.Convert(item.tg_status),
                            expedicao = se.Convert(item.tg_emitido),
                            matricula = item.st_matricula,
                            dtInicial = item.dt_inclusao != null ? Convert.ToDateTime(item.dt_inclusao).ToString("dd/MM/yyyy") : "",
                            dtUltExp = dtPrimTtrans != null ? Convert.ToDateTime(dtPrimTtrans).ToString("dd/MM/yyyy") : "",
                            tit = item.st_titularidade,
                            limM = mon.setMoneyFormat((long)item.vr_limiteMensal),
                            limT = mon.setMoneyFormat((long)item.vr_limiteTotal),
                            limCota = mon.setMoneyFormat((long)item.vr_extraCota),
                            dispM = mon.setMoneyFormat(dispM),
                            dispT = mon.setMoneyFormat(dispT),
                        });
                    }
                    else
                    {
                        res.Add(new CartaoListagemDTO
                        {
                            id = item.i_unique.ToString(),
                            associado = assoc.st_nome,
                            matricula = item.st_matricula,
                            limCota = mon.setMoneyFormat((long)item.vr_extraCota),
                            cartaoTitVia = item.st_matricula + "." +
                                            item.st_titularidade + ":" +
                                            item.nu_viaCartao,
                            selecionado = false,
                        });
                    }
                }
            }

            return Ok(new { count = query.Count(), results = res });
        }

        public IHttpActionResult Get(long id)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var cart = (from e in db.T_Cartao
                       where e.i_unique == id
                       select e).
                       FirstOrDefault();

            if (cart == null)
                return BadRequest();

            var prop = (from e in db.T_Proprietario
                         where e.i_unique == cart.fk_dadosProprietario
                         select e).
                         FirstOrDefault();

            var mon = new money();
            var se = new StatusExpedicao();
            var cs = new CartaoStatus();

            var lstDeps = new List<CartaoDependenteDTO>();

            foreach (var cartDep in (from e in db.T_Cartao
                                     where e.st_matricula == cart.st_matricula
                                     where e.st_empresa == cart.st_empresa
                                     where e.st_titularidade != "01"
                                     select e).
                                     ToList())
            {
                var propDep = (from e in db.T_Dependente
                               where e.fk_proprietario == cart.fk_dadosProprietario
                               where e.nu_titularidade.ToString().PadLeft(2, '0') == cartDep.st_titularidade
                               select e).
                               FirstOrDefault();

                lstDeps.Add(new CartaoDependenteDTO
                {
                    nome = propDep.st_nome,
                    tit = propDep.nu_titularidade.ToString(),
                    dt = ObtemData(cartDep.dt_inclusao)
                });
            }

            return Ok(new CartaoDTO
            {
                // dados proprietário
                nome = prop.st_nome,
                cpf = prop.st_cpf,
                tel = prop.st_telefone,
                email = prop.st_email,
                dtNasc = prop.dt_nasc != null ? Convert.ToDateTime(prop.dt_nasc).ToString("dd/MM/yyyy") : "",
                uf = prop.st_UF,
                cidade = prop.st_cidade,
                cep = prop.st_cep,
                end = prop.st_endereco,
                numero = prop.st_numero,
                bairro = prop.st_bairro,

                // cartão
                id = id.ToString(),
                matricula = cart.st_matricula,
                limMes = mon.setMoneyFormat((long)cart.vr_limiteMensal),
                limTot = mon.setMoneyFormat((long)cart.vr_limiteTotal),                
                vencMes = cart.st_venctoCartao == null ? "" : cart.st_venctoCartao.Substring(0, 2),
                vencAno = cart.st_venctoCartao == null ? "" : cart.st_venctoCartao.Substring(2, 2),
                banco = cart.st_banco,                
                bancoAg = cart.st_agencia,
                bancoCta = cart.st_conta,
                situacao = cs.Convert(cart.tg_status),
                expedicao = se.Convert(cart.tg_emitido),
                via = cart.nu_viaCartao.ToString(),
                lstDeps = lstDeps
            });
        }

        [NonAction]
        public void CopiaDadosCartao ( CartaoDTO mdl, 
                                       ref T_Cartao cart, 
                                       int? id_prop, 
                                       string st_empresa )
        {
            cart.fk_dadosProprietario = id_prop;
            cart.st_empresa = st_empresa;
            cart.st_matricula = mdl.matricula.PadLeft(6,'0');
            cart.st_banco = mdl.banco;
            cart.st_agencia = mdl.bancoAg;
            cart.st_conta = mdl.bancoCta;
            cart.st_venctoCartao = mdl.vencMes.PadLeft(2, '0') + mdl.vencAno.PadLeft(2, '0');
        }

        [NonAction]
        public void CopiaDadosProprietario(CartaoDTO mdl, ref T_Proprietario prop)
        {
            if (!string.IsNullOrEmpty(mdl.dtNasc))
                prop.dt_nasc = ObtemData(mdl.dtNasc);

            prop.st_nome = mdl.nome;
            prop.st_cpf = mdl.cpf;
            prop.st_telefone = mdl.tel;
            prop.st_email = mdl.email;
            prop.st_UF = mdl.uf;
            prop.st_cidade = mdl.cidade;
            prop.st_cep = mdl.cep;
            prop.st_endereco = mdl.end;
            prop.st_numero = mdl.numero;
            prop.st_bairro = mdl.bairro;
        }
        
        [HttpPost]
        public IHttpActionResult Post(CartaoDTO mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var st_empresa = userLoggedEmpresa;

            if ( (from e in db.T_Cartao
                  join p in db.T_Proprietario 
                    on e.fk_dadosProprietario equals (int) p.i_unique
                  where e.st_empresa == st_empresa
                  where p.st_cpf == mdl.cpf
                  select e).Any() )
            {
                return BadRequest("CPF já cadastrado nesta empresa!");
            }

            if ((from e in db.T_Cartao
                 where e.st_empresa == st_empresa
                 where e.st_matricula == mdl.matricula.PadLeft (6,'0')
                 select e).Any())
            {
                return BadRequest("Matrícula já cadastrada nesta empresa!");
            }

            var prop = new T_Proprietario();
            var cart = new T_Cartao();

            CopiaDadosProprietario(mdl, ref prop);
            
            var id_prop = Convert.ToInt32(db.InsertWithIdentity(prop));
            
            CopiaDadosCartao(mdl, ref cart, id_prop, st_empresa);

            cart.st_titularidade = "01";
            cart.nu_viaCartao = 1;

            cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);
            cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);
            cart.tg_tipoCartao = Convert.ToChar(TipoCartao.empresarial);
            cart.nu_senhaErrada = Convert.ToInt32(Context.NONE);

            cart.dt_bloqueio = DateTime.Now;
            cart.dt_inclusao = DateTime.Now;
            cart.dt_utlPagto = DateTime.Now;

            cart.vr_limiteMensal = 0;
            cart.vr_limiteTotal = 0;
            cart.vr_extraCota = 0;
            
            db.Insert(cart);

            // ----------------------------------
            // log de auditoria
            // ----------------------------------

            db.Insert(new LOG_Audit
            {
                tg_operacao = Convert.ToInt32(TipoOperacao.CadCartao),
                fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                dt_operacao = DateTime.Now,
                st_observacao = "",
                fk_generic = 0
            });

            return Ok();
        }
        
        [HttpPut]
        public IHttpActionResult Put(CartaoDTO mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var st_empresa = userLoggedEmpresa;

            if (mdl.modo == "altCotaGeral")
            {
                if (mdl.valor.Length == 0)
                    return BadRequest("Informe a cota extra corretamente!");

                var mats = mdl.array.TrimEnd(',').Split(',');

                var lst = (from e in db.T_Cartao
                           where e.st_empresa == st_empresa
                           where mats.Contains (e.st_matricula)
                           select e).
                           ToList();
                
                foreach (var _cart in lst)
                {
                    _cart.vr_extraCota = (int)ObtemValor(mdl.valor);

                    db.Update(_cart);

                    db.Insert(new LOG_Audit
                    {
                        tg_operacao = Convert.ToInt32(TipoOperacao.CotaExtraMensal),
                        fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                        dt_operacao = DateTime.Now,
                        st_observacao = "",
                        fk_generic = (int)_cart.i_unique
                    });
                }

                return Ok();
            }
            
            var cart = (from e in db.T_Cartao
                        where e.i_unique == Convert.ToInt32(mdl.id)
                        select e).
                        FirstOrDefault();

            if (cart == null)
                return BadRequest("Cartão inválido");

            if (cart.fk_dadosProprietario == null)
                return BadRequest("Cartão inválido");

            var prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();

            switch (mdl.modo)
            {
                case "altCota":
                    {
                        if (mdl.valor.Length == 0)
                            return BadRequest("Informe a cota extra corretamente!");

                        cart.vr_extraCota = (int)ObtemValor(mdl.valor);

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.CotaExtraMensal),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }

                case "altSenha":
                    {
                        if (mdl.valor.Length != 4)
                            return BadRequest("Senha requer 4 caracteres numéricos!");

                        cart.st_senha = DESCript(mdl.valor);

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.AlterSenha),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int) cart.i_unique
                        });
                        
                        return Ok();
                    }

                case "altLim":
                    {
                        if (mdl.valor.Length == 0)
                            return BadRequest("Informe os limites corretamente!");

                        if (!mdl.valor.Contains("|"))
                            return BadRequest("Informe os limites corretamente!");

                        var lst = mdl.valor.Split('|');
                        
                        cart.vr_limiteMensal = (int)ObtemValor(lst[0]);
                        cart.vr_limiteTotal = (int)ObtemValor(lst[1]);
                        
                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.AlterCartao),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }
                    
                case "altBloq":
                    {
                        cart.tg_status = Convert.ToChar(CartaoStatus.Bloqueado);
                        cart.dt_bloqueio = DateTime.Now;

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.BloqueioCartao),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }

                case "altDesbloq":
                    {
                        cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);
                        
                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.DesbloqueioCartao),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }

                case "altSegVia":
                    {
                        cart.nu_viaCartao = cart.nu_viaCartao + 1;
                        cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.ReqSegViaCart),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }

                case "criaDep":
                    {
                        int proxTit = (from e in db.T_Cartao
                                       where e.st_matricula == cart.st_matricula
                                       where e.st_empresa == cart.st_empresa
                                       select e).Count() + 1;

                        db.Insert(new T_Cartao
                        {
                            st_empresa = cart.st_empresa,
                            st_matricula = cart.st_matricula,
                            nu_viaCartao = 1,
                            st_titularidade = proxTit.ToString().PadLeft(2, '0'),
                            vr_limiteMensal = cart.vr_limiteMensal,
                            vr_limiteTotal = cart.vr_limiteTotal,
                            tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido),
                            tg_tipoCartao = Convert.ToChar(TipoCartao.empresarial),
                            tg_status = Convert.ToChar(CartaoStatus.Habilitado),
                            nu_senhaErrada = Convert.ToInt32(Context.NONE),
                            dt_bloqueio = DateTime.Now,
                            dt_inclusao = DateTime.Now,
                            dt_utlPagto = DateTime.Now,
                            fk_dadosProprietario = cart.fk_dadosProprietario,
                            st_venctoCartao = cart.st_venctoCartao,
                            st_banco = "",
                            st_agencia = "",
                            st_conta = "",
                            st_senha = cart.st_senha // copiar senha do proprietario
                        });

                        db.Insert(new T_Dependente
                        {
                            fk_proprietario = cart.fk_dadosProprietario,
                            nu_titularidade = proxTit,
                            st_nome = mdl.valor
                        });

                        db.Insert(new LOG_Audit
                        {
                            tg_operacao = Convert.ToInt32(TipoOperacao.CadDepenCart),
                            fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                            dt_operacao = DateTime.Now,
                            st_observacao = "",
                            fk_generic = (int)cart.i_unique
                        });

                        return Ok();
                    }
            }
            
            CopiaDadosProprietario(mdl, ref prop);

            db.Update(prop);

            CopiaDadosCartao(mdl, ref cart, (int?) prop.i_unique, st_empresa);

            db.Update(cart);

            // ----------------------------------
            // log de auditoria
            // ----------------------------------

            db.Insert(new LOG_Audit
            {
                tg_operacao = Convert.ToInt32(TipoOperacao.AltDadosPropCart),
                fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                dt_operacao = DateTime.Now,
                st_observacao = "",
                fk_generic = (int)cart.i_unique
            });

            db.Update(cart);

            return Ok();
        }
    }
}
