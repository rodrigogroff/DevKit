using LinqToDB;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class LojaListagemDTO
    {
        public string nome, 
                        razSoc,
                        cnpj,
                        telefone,
                        codigo,
                        end,
                        cidade,
                        estado,
                        txAdmin;
    }

    public class EmissoraLojaController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var nome = Request.GetQueryStringValue("nome");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var query = (from e in db.T_Loja
                         from conv in db.LINK_LojaEmpresa
                         where conv.fk_empresa == db.currentEmpresa.i_unique
                         where conv.fk_loja == e.i_unique
                         select e);

            if (!String.IsNullOrEmpty(nome))
            {
                query = (from e in query
                         where e.st_nome.ToUpper().Contains(nome)
                         select e);
            }

            var res = new List<LojaListagemDTO>();

            var page = query.Skip(skip).Take(take).ToList();

            var convenios = (from e in page
                             from conv in db.LINK_LojaEmpresa
                             where conv.fk_empresa == db.currentEmpresa.i_unique
                             select conv).
                             ToList();

            var mon = new money();

            foreach (var item in page)
            {
                var c = convenios.
                            Where(y => y.fk_loja == item.i_unique).
                                FirstOrDefault();

                res.Add(new LojaListagemDTO
                {
                    nome = item.st_nome,
                    razSoc = item.st_social,
                    cnpj = item.nu_CNPJ,
                    telefone = item.nu_telefone,
                    codigo = item.st_loja,
                    end = item.st_endereco.Replace ("{SE$3}",",").Replace("{SE$2}", ""),
                    cidade = item.st_cidade,
                    estado = item.st_estado,
                    txAdmin = mon.setMoneyFormat((long)c.tx_admin)
                });
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
