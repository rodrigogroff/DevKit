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
    public class AssociadoExtratoFuturoV1 : BaseService
    {
        public AssociadoExtratoFuturoV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoExtratoFuturo dto)
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

                    var lstCartao = repository.ObterListaCartao ( db, 
                                                                  associadoPrincipal.st_empresa, 
                                                                  associadoPrincipal.st_matricula).
                                            Select(y => (long)y.i_unique).
                                            ToList();

                    var lstTotParcs = repository.ObterListaParcelaDeListaCartaoSuperior(db, lstCartao, 2);

                    var lstIdsTrans = lstTotParcs.Select(y => (long) y.fk_log_transacoes).ToList();
                    var lstTotTransacoes = repository.ObterListaLogTransacao(db, lstIdsTrans);

                    int nuParcs = 1;

                    while (true)
                    {
                        dt = dt.AddMonths(1);
                        nuParcs++;

                        var lstParcs = lstTotParcs.Where(y => y.nu_parcela == nuParcs).ToList();

                        if (lstParcs.Count() == 0)
                            break;

                        long tot = 0;

                        foreach (var parc in lstParcs)
                        {
                            var logTrans = lstTotTransacoes.FirstOrDefault(y => y.i_unique == parc.fk_log_transacoes);
                            if (logTrans == null) continue;
                            if (logTrans.tg_confirmada.ToString() == TipoConfirmacao.Confirmada)
                            {
                                tot += (long) parc.vr_valor;
                            }
                        }   

                        dto.parcelamento.Add(new AssociadoExtratoFuturoSintetico
                        {
                            mesAno = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year,
                            vrDisponivel = mon.setMoneyFormat((long)associadoPrincipal.vr_limiteMensal - tot),
                            valor = mon.setMoneyFormat(tot),
                            pctComprometido = mon.setMoneyFormat( (10000 * tot) / (long) associadoPrincipal.vr_limiteMensal )
                        });
                    }
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
