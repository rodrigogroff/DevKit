﻿using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using SyCrafEngine;

namespace DevKit.Web.Controllers
{
    public class RelAssociadosItem
    {
        public string associado, cartao, cpf, dispM, limM, dispT, limT, tit;
    }

    public class RelAssociadosController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var matricula = Request.GetQueryStringValue("matricula");
            var skip = Request.GetQueryStringValue<int>("skip");
            var take = Request.GetQueryStringValue<int>("take");
            var idEmpresa = Request.GetQueryStringValue<int?>("idEmpresa", null);
            var bloqueado = Request.GetQueryStringValue<bool?>("bloqueado");

            if (!StartDatabaseAndAuthorize())
                return BadRequest();

            var stEmpresa = "";

            if (idEmpresa != null)
            {
                stEmpresa = (from e in db.T_Empresa
                             where (int)e.i_unique == idEmpresa
                             select e).
                             FirstOrDefault().
                             st_empresa;
            }

            var query = (from e in db.T_Cartao select e);

            if (bloqueado != null)
            {
                if (bloqueado == false)
                    query = (from e in query
                             where e.tg_status.ToString() == "0"
                             select e);
                else
                    query = (from e in query
                             where e.tg_status.ToString() == "1"
                             select e);
            }

            if (busca != null)
            {
                query = (from e in query
                         join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int) associado.i_unique
                         where associado.st_nome.Contains(busca)
                         select e);
            }

            if (matricula != null && matricula != "")
            {
                query = (from e in query
                         where e.st_matricula.Contains(matricula)
                         select e);
            }

            if (stEmpresa != "")
            {
                query = (from e in query
                         where e.st_empresa == stEmpresa
                         select e);                     
            }

            var res = new List<RelAssociadosItem>();

            query = (from e in query
                     join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int)associado.i_unique
                     orderby associado.st_nome
                     select e);

            var calcAcesso = new CodigoAcesso();

            var sd = new SaldoDisponivel();
            var mon = new money();

            foreach (var item in query.Skip(skip).Take(take).ToList())
            {
                var assoc = (from e in db.T_Proprietario
                             where e.i_unique == item.fk_dadosProprietario
                             select e).
                             FirstOrDefault();

                long dispM = 0, dispT = 0;

                sd.Obter(db, item, ref dispM, ref dispT);

                if (assoc != null)
                {
                    var codAcessoCalc = calcAcesso.Obter(  item.st_empresa,
                                                           item.st_matricula,
                                                           item.st_titularidade,
                                                           item.nu_viaCartao,
                                                           assoc.st_cpf);

                    res.Add(new RelAssociadosItem
                    {
                        associado = assoc.st_nome,

                        cartao = item.st_empresa + "." +
                                 item.st_matricula + "." +
                                 codAcessoCalc + "." +
                                 item.st_venctoCartao,

                        cpf = assoc.st_cpf,
                        tit = item.st_titularidade,
                        dispM = mon.setMoneyFormat(dispM),
                        dispT = mon.setMoneyFormat(dispT),
                        limM = mon.setMoneyFormat((long)item.vr_limiteMensal),
                        limT = mon.setMoneyFormat((long)item.vr_limiteTotal),
                    });
                }
                    
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
