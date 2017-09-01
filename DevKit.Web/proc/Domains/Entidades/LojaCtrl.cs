using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using System;

namespace DevKit.Web.Controllers
{
    public class LojaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var terminal = Request.GetQueryStringValue("terminal");
            var cidade = Request.GetQueryStringValue("cidade");
            var estado = Request.GetQueryStringValue("estado");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
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

            var lst = new List<Loja>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                lst.Add(new Loja
                {
                    id = item.i_unique.ToString(),
                    terminal = item.st_loja,
                    nome = item.st_nome,
                    cidade = item.st_cidade,
                    estado = item.st_estado,
                    situacao = item.tg_blocked == '1' ? "Bloqueada" : "Ativa",
                    tipoVenda = item.tg_portalComSenha == 1 ? "Com Senha" : "Sem senha",
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

            return Ok(new Loja
            {
                id = mdl.i_unique.ToString(),
                terminal = mdl.st_loja,
                nome = mdl.st_nome,
                cidade = mdl.st_cidade,
                estado = mdl.st_estado,
                tg_blocked = mdl.tg_blocked.ToString(),
                tg_portalComSenha = mdl.tg_portalComSenha != 1 ? "0" : "1"
            });
        }

        [HttpPut]
        public IHttpActionResult Post(Loja mdl)
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

            return Ok();
        }
    }
}
