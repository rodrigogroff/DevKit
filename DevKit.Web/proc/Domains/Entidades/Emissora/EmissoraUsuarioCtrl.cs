using LinqToDB;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Security.Cryptography;
using System.Text;

namespace DevKit.Web.Controllers
{
    public class UsuarioListagemDTO
    {
        public string   id,
                        nome,
                        situacao;
    }

    public class UsuarioDTO
    {
        public string modo, valor, id, nome;
    }

    public class EmissoraUsuarioController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var nome = Request.GetQueryStringValue("nome","").ToUpper();
            
            var skip = Request.GetQueryStringValue<int>("skip", 0);
            var take = Request.GetQueryStringValue<int>("take", 1);
            
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Usuario
                         where e.st_empresa == userLoggedEmpresa
                         select e);

            if (!String.IsNullOrEmpty(nome))
            {
                query = (from e in query
                         where e.st_nome.ToUpper().Contains(nome)
                         select e);
            }

            var res = new List<UsuarioListagemDTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var sit = "Ativo";

                if (item.tg_bloqueio.ToString() == "1")
                    sit = "Bloqueado";
                else if (item.tg_bloqueio.ToString() == "2")
                    sit = "Cancelado";

                res.Add(new UsuarioListagemDTO
                {
                    id = item.i_unique.ToString(),
                    nome = item.st_nome,
                    situacao = sit
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }

        /*
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
        */

        public string getMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        [HttpPut]
        public IHttpActionResult Put(UsuarioDTO mdl)
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var st_empresa = userLoggedEmpresa;
            
            var usuario = (from e in db.T_Usuario
                        where e.i_unique == Convert.ToInt32(mdl.id)
                        select e).
                        FirstOrDefault();

            if (usuario == null)
                return BadRequest("Usuário inválido");

            switch (mdl.modo)
            {
                case "altSenhaUsuario":
                    {
                        if (mdl.valor.Length != 4)
                            return BadRequest("Senha requer 4 caracteres numéricos!");

                        usuario.st_senha = getMd5Hash(mdl.valor);

                        db.Update(usuario);

                        return Ok();
                    }
            }

            db.Update(usuario);
            
            return Ok();
        }
    }
}
