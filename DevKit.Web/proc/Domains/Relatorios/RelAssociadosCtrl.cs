using LinqToDB;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using SyCrafEngine;
using DataModel;
using System;

namespace DevKit.Web.Controllers
{
    public class RelAssociadosItem
    {
        public string associado, cartao, cpf, dispM, limM, dispT, limT, tit;
        public string via, status, exped, dt_exp, dt_ultMov;
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
            var expedicao = Request.GetQueryStringValue("expedicao");

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

            if (expedicao != null)
            {
                if (expedicao == "R")
                {
                    query = (from e in query where e.tg_emitido.ToString() == StatusExpedicao.NaoExpedido select e);
                }
                else if (expedicao == "G")
                {
                    query = (from e in query where e.tg_emitido.ToString() == StatusExpedicao.EmExpedicao select e);
                }
                else if (expedicao == "A")
                {
                    query = (from e in query where e.tg_emitido.ToString() == StatusExpedicao.Expedido select e);
                }
            }

            if (busca != null)
            {
                query = (from e in query
                         join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int) associado.i_unique
                         where associado.st_nome.ToUpper().Contains(busca.ToUpper())
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

                    var exped = "Requerido";

                    if (item.tg_emitido.ToString() == StatusExpedicao.EmExpedicao)                    
                        exped = "Gráfica";                    
                    else if (item.tg_emitido.ToString() == StatusExpedicao.Expedido)                    
                        exped = "Expedido";

                    var ultLoteDet = db.T_LoteCartaoDetalhe.
                        Where(y => y.fk_cartao == item.i_unique).
                        OrderByDescending(y => y.i_unique).FirstOrDefault();

                    var ultMov = db.LOG_Transacoes.
                                    Where(y => y.fk_cartao == item.i_unique && y.tg_confirmada.ToString() == TipoConfirmacao.Confirmada).
                                        OrderByDescending(y => y.dt_transacao).
                                        FirstOrDefault();

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
                        via = item.nu_viaCartao.ToString(),
                        status = item.tg_status.ToString() == CartaoStatus.Habilitado ? "Habilitado" : "Bloqueado",
                        exped = exped,
                        dt_exp = ultLoteDet != null ? ultLoteDet.dt_ativacao != null ? Convert.ToDateTime(ultLoteDet.dt_ativacao).ToString("dd/MM/yyyy HH:mm") : "" : "",
                        dt_ultMov = ultMov != null ? Convert.ToDateTime(ultMov.dt_transacao).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }
                    
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
