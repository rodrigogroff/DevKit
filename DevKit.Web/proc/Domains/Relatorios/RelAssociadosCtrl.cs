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
        public string   id, associado, cartao, cpf, dispM, limM, dispT, limT, tit,
                        via, status, exped, dt_exp, dt_pedido, dt_ultMov;
    }

    public class RelAssociadosController : ApiControllerBase
    {
        public IHttpActionResult Get()
        {
            var busca = Request.GetQueryStringValue("busca");
            var matricula = Request.GetQueryStringValue("matricula");
            var titularidade = Request.GetQueryStringValue("titularidade");
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
            else
            {
                // vem da empresa logada
                stEmpresa = userLoggedEmpresa;
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

            if (!string.IsNullOrEmpty (titularidade))
            {
                query = (from e in query
                         where e.st_titularidade == titularidade.PadLeft(2, '0')
                         select e);
            }

            int qtdDeps = 0; 

            if (busca != null)
            {
                var lstFksAssociados = db.T_Proprietario.Where(y => y.st_nome.ToUpper().Contains(busca.ToUpper())).Select(y => y.i_unique).ToList();

                var lstCarts = new List<decimal>();

                if (lstFksAssociados.Count() > 0)
                {
                    foreach (var item in lstFksAssociados)
                    {
                        lstCarts.Add(
                            db.T_Cartao.FirstOrDefault(
                                y => y.fk_dadosProprietario == item &&
                                     Convert.ToInt32(y.st_titularidade) == 1).
                                     i_unique);
                    }                    
                }

                var lstFksDependentes = db.T_Dependente.Where(y => y.st_nome.ToUpper().Contains(busca.ToUpper())).ToList();

                if (lstFksDependentes.Count() > 0)
                {
                    qtdDeps = lstFksDependentes.Count();

                    foreach (var item in lstFksDependentes)
                    {
                        lstCarts.Add ( 
                            db.T_Cartao.FirstOrDefault ( 
                                y => y.fk_dadosProprietario == item.fk_proprietario && 
                                     Convert.ToInt32(y.st_titularidade) == item.nu_titularidade).
                                     i_unique);
                    }                    
                }

                if (lstCarts.Any())
                    query = query.Where(y => lstCarts.Contains(y.i_unique));
            }

            if (matricula != null && matricula != "")
            {
                query = (from e in query
                         where e.st_matricula == matricula.PadLeft(6, '0')
                         select e);
            }

            if (!string.IsNullOrEmpty(stEmpresa))
            {
                query = (from e in query
                         where e.st_empresa == stEmpresa
                         select e);                     
            }

            var res = new List<RelAssociadosItem>();

            if (qtdDeps == 0)
                query = (from e in query
                         join emp in db.T_Empresa on e.st_empresa equals emp.st_empresa
                         join associado in db.T_Proprietario on e.fk_dadosProprietario equals (int)associado.i_unique
                         where emp.tg_bloq == 0
                         orderby associado.st_nome
                         select e);

            var calcAcesso = new CodigoAcesso();

            var sd = new SaldoDisponivel();
            var mon = new money();

            var lstFinal = query.Skip(skip).Take(take).ToList();

            foreach (var item in lstFinal)
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

                    var nome = "";

                    if (item.st_titularidade == "01")
                        nome = assoc.st_nome;
                    else
                        nome = db.T_Dependente.FirstOrDefault(y => y.fk_proprietario == assoc.i_unique).st_nome;
                    
                    res.Add(new RelAssociadosItem
                    {
                        id = item.i_unique.ToString(),
                        associado = nome,

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
                        dt_pedido = ultLoteDet != null ? ultLoteDet.dt_pedido != null ? Convert.ToDateTime(ultLoteDet.dt_pedido).ToString("dd/MM/yyyy HH:mm") : "" : "",
                        dt_exp = ultLoteDet != null ? ultLoteDet.dt_ativacao != null ? Convert.ToDateTime(ultLoteDet.dt_ativacao).ToString("dd/MM/yyyy HH:mm") : "" : "",
                        dt_ultMov = ultMov != null ? Convert.ToDateTime(ultMov.dt_transacao).ToString("dd/MM/yyyy HH:mm") : ""
                    });
                }                    
            }

            return Ok(new { count = query.Count(), results = res });
        }
    }
}
