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
                        limAcc,
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
                        limMes, limTot, limCota, limAcc,
                        banco, bancoAg, bancoCta,
                        tel,
                        email,
                        situacao, expedicao,
                        via,
                        fkEmpresa,                      
                        uf, cidade, cep, end, numero, bairro,
                        modo, valor, array;

        public bool? tg_convenioComSaldo;

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
                         where e.st_titularidade == "01"
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

                    if (matricula != null && matricula != "")
                        query = (from e in query
                             join p in db.T_Proprietario on e.fk_dadosProprietario equals (int)p.i_unique
                             orderby e.st_titularidade, p.st_nome
                             select e);

                    break;

                case EnumOrdemEmissorManutCartoes.matricula:

                    if (matricula != null && matricula != "")
                        query = (from e in query
                             orderby e.st_titularidade, e.st_matricula
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

                            if (cartTit == null)
                                continue;

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
                            limAcc = cartaoAtual.vr_saldoConvenio > 0 ? mon.setMoneyFormat((long)cartaoAtual.vr_saldoConvenio) : "0,00",
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

            var cart = (from e in db.T_Cartao where e.i_unique == id select e).FirstOrDefault();

            if (cart == null)
                return BadRequest();
            
            db.Insert(new LOG_Audit
            {
                dt_operacao = DateTime.Now,
                fk_usuario = userIdLoggedUsuario,
                st_empresa = userLoggedEmpresa,
                st_oper = "Visualizar Cartão",
                st_log = "Mat: " + cart.st_matricula 
            });

            var t_Emp = (from e in db.T_Empresa where e.st_empresa == cart.st_empresa select e).FirstOrDefault();

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
                fkEmpresa = t_Emp.i_unique.ToString(),
                // cartão
                id = id.ToString(),
                matricula = cart.st_matricula,
                tg_convenioComSaldo = cart.tg_convenioComSaldo,
                limMes = mon.setMoneyFormat((long)cart.vr_limiteMensal),
                limTot = mon.setMoneyFormat((long)cart.vr_limiteTotal),                
                limAcc = cart.vr_saldoConvenio > 0 ? mon.setMoneyFormat((long)cart.vr_saldoConvenio) : "0,00",
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

            if (string.IsNullOrEmpty(cart.st_empresa))
                cart.st_empresa = st_empresa;

            if (string.IsNullOrEmpty(cart.st_matricula))
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

            var prop = new T_Proprietario();
            var cart = new T_Cartao();

            var st_empresa = userLoggedEmpresa;

            if (!string.IsNullOrEmpty(mdl.fkEmpresa))
            {
                // dba
                var tDb = db.T_Empresa.FirstOrDefault(y => y.i_unique.ToString() == mdl.fkEmpresa);

                st_empresa = tDb.st_empresa;

                cart.tg_convenioComSaldo = tDb.tg_convenioComSaldo;
            }

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

            CopiaDadosProprietario(mdl, ref prop);
            
            var id_prop = Convert.ToInt32(db.InsertWithIdentity(prop));
            
            CopiaDadosCartao(mdl, ref cart, id_prop, st_empresa);

            cart.st_titularidade = "01";
            cart.nu_viaCartao = 1;

            cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);
            cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);
            cart.tg_tipoCartao = Convert.ToChar(TipoCartao.empresarial);
            cart.nu_senhaErrada = Convert.ToInt32(Context.NONE);

            cart.tg_convenioComSaldo = mdl.tg_convenioComSaldo;

            cart.dt_bloqueio = DateTime.Now;
            cart.dt_inclusao = DateTime.Now;
            cart.dt_utlPagto = DateTime.Now;

            cart.vr_limiteMensal = LimpaValor(mdl.limMes);
            cart.vr_limiteTotal = LimpaValor(mdl.limTot);
            cart.vr_extraCota = 0;

            cart.i_unique = Convert.ToDecimal (db.InsertWithIdentity(cart));

            // ----------------------------------
            // log de auditoria
            // ----------------------------------

            db.Insert(new LOG_Audit
            {
                dt_operacao = DateTime.Now,
                fk_usuario = userIdLoggedUsuario,
                st_empresa = userLoggedEmpresa,
                st_oper = "Novo Cartão",
                st_log = "Mat: " + cart.st_matricula + " Nome:" + prop.st_nome
            });


            return Ok(cart);
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
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Cota Extra ",
                            st_log = "Mat: " + cart.st_matricula
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
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Alteração de senha ",
                            st_log = "Mat: " + cart.st_matricula
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

                        string de_para = " antigo mensal: " + cart.vr_limiteMensal + " antigo total: " + cart.vr_limiteMensal;

                        cart.vr_limiteMensal = LimpaValor(lst[0]);
                        cart.vr_limiteTotal = LimpaValor(lst[1]);

                        de_para += " novo mensal: " + cart.vr_limiteMensal + " novo total: " + cart.vr_limiteMensal;
                        
                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Alteração de limite ",
                            st_log = "Mat: " + cart.st_matricula + " => " + de_para
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
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Bloqueio ",
                            st_log = "Mat: " + cart.st_matricula 
                        });

                        return Ok();
                    }

                case "altDesbloq":
                    {
                        cart.tg_status = Convert.ToChar(CartaoStatus.Habilitado);
                        
                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Desbloqueio ",
                            st_log = "Mat: " + cart.st_matricula
                        });

                        return Ok();
                    }

                case "altSegVia":
                    {
                        if (cart.tg_status.ToString() == CartaoStatus.Bloqueado)
                            return BadRequest("Cartão Bloqueado!");

                        cart.nu_viaCartao = cart.nu_viaCartao + 1;
                        cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);

                        db.Update(cart);

                        db.Insert(new LOG_Audit
                        {
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Segunda via ",
                            st_log = "Mat: " + cart.st_matricula
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
                            dt_operacao = DateTime.Now,
                            fk_usuario = userIdLoggedUsuario,
                            st_empresa = userLoggedEmpresa,
                            st_oper = "Cartão / Novo Dependente ",
                            st_log = "Mat: " + cart.st_matricula + " tit " + proxTit.ToString().PadLeft(2, '0') + " => Nome: " + mdl.valor
                        });
                                               
                        return Ok();
                    }
            }
            
            CopiaDadosProprietario(mdl, ref prop);

            db.Update(prop);

            CopiaDadosCartao(mdl, ref cart, (int?) prop.i_unique, st_empresa);

            // custom field
            cart.tg_convenioComSaldo = mdl.tg_convenioComSaldo;

            db.Update(cart);

            db.Insert(new LOG_Audit
            {
                dt_operacao = DateTime.Now,
                fk_usuario = userIdLoggedUsuario,
                st_empresa = userLoggedEmpresa,
                st_oper = "Editar Dados Cartão",
                st_log = "Mat: " + cart.st_matricula
            });

            db.Update(cart);

            return Ok();
        }
    }
}
