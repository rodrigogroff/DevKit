using LinqToDB;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using SyCrafEngine;
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
                        cartao, 
                        cpf, 
                        tit, 
                        dispM, 
                        limM, 
                        dispT, 
                        limT;             
    }

    public class CartaoDTO
    {
        public string id,
                        matricula,
                        nome,
                        cpf,
                        vencMes, vencAno,
                        dtNasc,
                        limMes, limTot,
                        banco, bancoAg, bancoCta,
                        tel,
                        email,
                        situacao, expedicao,
                        via,
                        uf, cidade, cep, end, numero, bairro,
                        modo, valor;
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

            var idSit = Request.GetQueryStringValue("idSit");
            var idExp = Request.GetQueryStringValue("idExp");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Cartao
                         where e.st_empresa == userLoggedEmpresa
                         select e);

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

            if (!String.IsNullOrEmpty(idExp))
            {
                query = (from e in query
                         where e.tg_emitido.ToString() == idExp
                         select e);
            }

            if (!String.IsNullOrEmpty(idSit))
            {
                query = (from e in query
                         where e.tg_status.ToString() == idSit
                         select e);
            }

            if (matricula != null && matricula != "")
            {
                query = (from e in query
                         where e.st_matricula == matricula.PadLeft(6, '0')
                         select e);
            }

            query = (from e in query
                     orderby e.st_matricula
                     select e);

            var res = new List<CartaoListagemDTO>();

            query = (from e in query
                     join associado in db.T_Proprietario 
                          on e.fk_dadosProprietario equals (int)associado.i_unique
                     orderby associado.st_nome
                     select e);

            var calcAcesso = new CodigoAcesso();

            var mon = new money();
            var se = new StatusExpedicao();
            var cs = new CartaoStatus();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in db.T_Proprietario
                             where e.i_unique == item.fk_dadosProprietario
                             select e).
                             FirstOrDefault();
                
                if (assoc != null)
                {
                    var codAcessoCalc = calcAcesso.Obter(item.st_empresa,
                                                           item.st_matricula,
                                                           item.st_titularidade,
                                                           item.nu_viaCartao,
                                                           assoc.st_cpf);

                    res.Add(new CartaoListagemDTO
                    {
                        id = item.i_unique.ToString(),
                        associado = assoc.st_nome,
                        cartao = item.st_empresa + "." +
                                 item.st_matricula + "." +
                                 codAcessoCalc + "." +
                                 item.st_venctoCartao,
                        cpf = assoc.st_cpf,
                        situacao = cs.Convert(item.tg_status),
                        expedicao = se.Convert(item.tg_emitido),
                        matricula = item.st_matricula,
                        tit = item.st_titularidade,
                        limM = mon.setMoneyFormat((long)item.vr_limiteMensal),
                        limT = mon.setMoneyFormat((long)item.vr_limiteTotal),
                    });
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
                return StatusCode(HttpStatusCode.NotFound);

            var prop = (from e in db.T_Proprietario
                         where e.i_unique == cart.fk_dadosProprietario
                         select e).
                         FirstOrDefault();

            var mon = new money();
            var se = new StatusExpedicao();
            var cs = new CartaoStatus();

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
            cart.vr_limiteMensal = (int)ObtemValor(mdl.limMes);
            cart.vr_limiteTotal = (int)ObtemValor(mdl.limTot);
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
            cart.tg_emitido = Convert.ToInt32(StatusExpedicao.NaoExpedido);
            cart.tg_tipoCartao = Convert.ToChar(TipoCartao.empresarial);
            cart.nu_senhaErrada = Convert.ToInt32(Context.NONE);
            
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

            var cart = (from e in db.T_Cartao
                        where e.i_unique == Convert.ToInt32(mdl.id)
                        select e).
                        FirstOrDefault();

            var prop = (from e in db.T_Proprietario
                        where e.i_unique == cart.fk_dadosProprietario
                        select e).
                        FirstOrDefault();

            switch (mdl.modo)
            {
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
                            fk_generic = 0
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
                fk_generic = 0
            });

            db.Update(cart);

            return Ok();
        }
    }
}
