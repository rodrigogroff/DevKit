using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Entities.Enums;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Service
{
    public class AssociadoExtratoAtualV1 : BaseService
    {
        public AssociadoExtratoAtualV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoExtratoAtual dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var associadoPrincipal = repository.ObterCartao(db, au._id);
                    var empresa = repository.ObterEmpresa(db, associadoPrincipal.st_empresa);

                    var mon = new money();

                    long dispMensal = 0,
                            dispTotal = 0,
                            vrUtilizadoAtual = 0;

                    new BuscaSaldo().SaldoCartao(repository,
                                                   db,
                                                   associadoPrincipal,
                                                   ref dispMensal,
                                                   ref dispTotal,
                                                   ref vrUtilizadoAtual);

                    var dt = DateTime.Now;

                    if (DateTime.Now.Day >= empresa.nu_diaFech)
                        dt = dt.AddMonths(1);

                    dto.mesAtual = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year;
                    dto.saldoDisponivel = "R$ " + mon.setMoneyFormat(dispMensal);

                    var lst = repository.ObterListaCartao(db, associadoPrincipal.st_empresa, associadoPrincipal.st_matricula).
                                Select ( y=> (long) y.i_unique ).
                                ToList();

                    var parcs = repository.ObterListaParcelaDeListaCartaoIgual(db, lst, 1);

                    var lstFKTrans = parcs.Select(y => (long)y.fk_log_transacoes).Distinct().ToList();
                    var lstTrans = repository.ObterListaLogTransacao(db, lstFKTrans);

                    var lstFKLojas = parcs.Select(y => (long) y.fk_loja).Distinct().ToList();
                    var lstLoja = repository.ObterListaLoja(db, lstFKLojas);
                    
                    long totAtual = 0;

                    foreach (var item in parcs)
                    {
                        var ltr = lstTrans.FirstOrDefault(y => y.i_unique == item.fk_log_transacoes);

                        if (ltr == null) continue;
                        if (ltr.tg_confirmada.ToString() != TipoConfirmacao.Confirmada) continue;

                        totAtual += (long) item.vr_valor;

                        dto.vendas.Add(new AssociadoExtratoVenda
                        {
                            dtVenda = Convert.ToDateTime(item.dt_inclusao).ToString("dd/MM/yyyy HH:mm"),
                            nsu = item.nu_nsu.ToString(),
                            parcela = item.nu_parcela + " / " + item.nu_tot_parcelas,
                            valor = mon.setMoneyFormat((long)item.vr_valor),
                            estab = lstLoja.FirstOrDefault(y=> y.i_unique == item.fk_loja).st_nome,
                        });
                    }

                    dto.totalExtrato = "R$ " + mon.setMoneyFormat(totAtual);
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = new ServiceError
                {
                    message = _defaultError,
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }
    }
}
