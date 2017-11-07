using System.Collections.Generic;
using System.Web.Http;
using System;
using System.Linq;
using LinqToDB;
using SyCrafEngine;
using DataModel;

namespace DevKit.Web.Controllers
{
    public class LojistaDTO
    {
        public string nome = "", cidade = "", estado = "", endereco = "";
    }
    
    public class LojistasAssociadoController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            if (!StartDatabaseAndAuthorize())
                return BadRequest("Não autorizado!");

            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var busca = Request.GetQueryStringValue("busca");
                        
            var query = from e in db.T_Loja
                        join lemp in db.LINK_LojaEmpresa on e.i_unique equals (long)lemp.fk_loja
                        where lemp.fk_empresa == db.currentAssociadoEmpresa.i_unique
                        select e;

            if (busca!= null)
            {
                query = from e in query
                        where e.st_nome.ToUpper().Contains(busca)
                        select e;
            }

            query = from e in query
                    orderby e.st_nome
                    select e;

            var list = new List<LojistaDTO>();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                list.Add(new LojistaDTO
                {
                    nome = item.st_nome,
                    cidade = item.st_cidade,
                    estado = item.st_estado,
                    endereco = item.st_endereco.Replace ("{SE$3}",",")
                });
            }

            return Ok(new
            {
                count = query.Count(),
                results = list
            });
        }
    }
}
