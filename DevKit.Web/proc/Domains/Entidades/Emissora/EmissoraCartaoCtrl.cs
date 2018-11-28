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

    public class CartaoLoteDTO
    {
        public string codigo, dtCriacao, dtGrafica, dtAtivacao;
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
        public List<CartaoLoteDTO> lstLotes = new List<CartaoLoteDTO>();
    }

    public class EmissoraCartaoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var idEmpresa = Request.GetQueryStringValue<long?>("idEmpresa", null);

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

            var tEmp = db.currentEmpresa;

            if (idEmpresa != null)
            {
                tEmp = db.T_Empresa.FirstOrDefault(y => y.i_unique == idEmpresa);

                if (tEmp == null)
                    return BadRequest();
            }

            var query = (from e in db.T_Cartao
                         where e.st_empresa == tEmp.st_empresa
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

            foreach (var cartaoAtual in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in lstAssoc
                             where e.i_unique == cartaoAtual.fk_dadosProprietario
                             select e).
                             FirstOrDefault();
                
                if (assoc != null)
                {
                    long dispM = 0, dispT = 0;

                    if (!listagemCota)
                        sd.Obter(db, cartaoAtual, ref dispM, ref dispT);

                    if (cartaoAtual.vr_extraCota == null) cartaoAtual.vr_extraCota = 0;
                    if (cartaoAtual.vr_limiteMensal == null) cartaoAtual.vr_limiteMensal = 0;
                    if (cartaoAtual.vr_limiteTotal == null) cartaoAtual.vr_limiteTotal = 0;

                    string colorBack = "", colorFront = "";

                    if (cartaoAtual.tg_status.ToString() == CartaoStatus.Bloqueado)
                    {
                        colorBack = "white";
                        colorFront = "black";
                    }

                    DateTime? dtPrimTtrans = null;

                    if (!listagemCota)
                        dtPrimTtrans = (from e in db.LOG_Transacoes
                                        where e.fk_cartao == cartaoAtual.i_unique
                                        select e.dt_transacao).
                                        FirstOrDefault();

                    if (!listagemCota)
                    {
                        var limM = cartaoAtual.vr_limiteMensal;
                        var limT = cartaoAtual.vr_limiteTotal;
                        var limEC = cartaoAtual.vr_extraCota;
                        var assocNome = assoc.st_nome;

                        //dependente
                        if (cartaoAtual.st_titularidade != "01")
                        {
                            var cartTit = (from e in db.T_Cartao
                                       where e.st_titularidade == "01"
                                       where e.st_matricula == cartaoAtual.st_matricula
                                       where e.st_empresa == cartaoAtual.st_empresa
                                       select e).FirstOrDefault();

                             limM = cartTit.vr_limiteMensal;
                             limT = cartTit.vr_limiteTotal;
                             limEC = cartTit.vr_extraCota;

                            var depDados = (from e in db.T_Dependente
                                            where e.fk_proprietario == cartaoAtual.fk_dadosProprietario
                                            select e).
                                           FirstOrDefault();

                            if (depDados != null)
                                assocNome = depDados.st_nome;
                        }

                        res.Add(new CartaoListagemDTO
                        {
                            colorBack = colorBack,
                            colorFront = colorFront,
                            id = cartaoAtual.i_unique.ToString(),
                            via = cartaoAtual.nu_viaCartao.ToString(),
                            associado = assocNome,
                            cartaoTitVia = cartaoAtual.st_matricula + "." +
                                            cartaoAtual.st_titularidade + ":" +
                                            cartaoAtual.nu_viaCartao,
                            cpf = assoc.st_cpf,
                            situacao = cs.Convert(cartaoAtual.tg_status),
                            expedicao = se.Convert(cartaoAtual.tg_emitido),
                            matricula = cartaoAtual.st_matricula,
                            dtInicial = cartaoAtual.dt_inclusao != null ? Convert.ToDateTime(cartaoAtual.dt_inclusao).ToString("dd/MM/yyyy") : "",
                            dtUltExp = dtPrimTtrans != null ? Convert.ToDateTime(dtPrimTtrans).ToString("dd/MM/yyyy") : "",
                            tit = cartaoAtual.st_titularidade,
                            limM = mon.setMoneyFormat((long)limM),
                            limT = mon.setMoneyFormat((long)limT),
                            limCota = mon.setMoneyFormat((long)limEC),
                            dispM = mon.setMoneyFormat(dispM),
                            dispT = mon.setMoneyFormat(dispT),
                        });
                    }
                    else
                    {
                        res.Add(new CartaoListagemDTO
                        {
                            id = cartaoAtual.i_unique.ToString(),
                            associado = assoc.st_nome,
                            matricula = cartaoAtual.st_matricula,
                            limCota = mon.setMoneyFormat((long)cartaoAtual.vr_extraCota),
                            cartaoTitVia = cartaoAtual.st_matricula + "." +
                                            cartaoAtual.st_titularidade + ":" +
                                            cartaoAtual.nu_viaCartao,
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

            var lstLotes = new List<CartaoLoteDTO>();

            foreach (var item in db.T_LoteCartaoDetalhe.
                                        Where ( y=> y.fk_cartao == id).
                                        OrderByDescending ( y=> y.i_unique )
                                        .ToList())
            {
                var loteTb = db.T_LoteCartao.FirstOrDefault(y => y.i_unique == item.fk_lote);

                if (loteTb != null)
                {
                    var registro = new CartaoLoteDTO
                    {
                        codigo = loteTb.i_unique.ToString(),

                        dtCriacao = loteTb.dt_abertura != null ?
                            Convert.ToDateTime(loteTb.dt_abertura).ToString("dd/MM/yyyy HH:mm") : "",

                        dtGrafica = loteTb.dt_envio_grafica != null ? 
                            Convert.ToDateTime(loteTb.dt_envio_grafica).ToString("dd/MM/yyyy HH:mm") : "",
                    };

                    // lote inteiro ativado
                    if (item.dt_ativacao == null && loteTb.dt_ativacao != null) 
                    {
                        registro.dtAtivacao = 
                            Convert.ToDateTime(loteTb.dt_ativacao).ToString("dd/MM/yyyy HH:mm");
                    }

                    // manual
                    else if (item.dt_ativacao != null) 
                    {
                        registro.dtAtivacao =
                            Convert.ToDateTime(item.dt_ativacao).ToString("dd/MM/yyyy HH:mm");
                    }

                    lstLotes.Add(registro);
                }                
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

                // listas
                lstDeps = lstDeps,
                lstLotes = lstLotes
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

            if (prop.dt_nasc == null)
                prop.dt_nasc = new DateTime(1976, 1, 1);

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

            cart.vr_limiteMensal = LimpaValor(mdl.limMes);
            cart.vr_limiteTotal = LimpaValor(mdl.limTot);
            cart.vr_extraCota = 0;
            
            db.Insert(cart);

            // ----------------------------------
            // log de auditoria
            // ----------------------------------

            /*db.Insert(new LOG_Audit
            {
                tg_operacao = Convert.ToInt32(TipoOperacao.CadCartao),
                fk_usuario = Convert.ToInt32(userLoggedEmpresaIdUsuario),
                dt_operacao = DateTime.Now,
                st_observacao = "",
                fk_generic = 0
            });*/

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
                        
                        cart.vr_limiteMensal = LimpaValor(lst[0]);
                        cart.vr_limiteTotal = LimpaValor(lst[1]);
                        
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
