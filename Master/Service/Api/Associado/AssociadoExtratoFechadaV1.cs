using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Entities.Enums;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Service
{
    public class AssociadoExtratoFechadaV1 : BaseService
    {
        public AssociadoExtratoFechadaV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoExtratoFechada dto)
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

                    var dt = new DateTime ( DateTime.Now.Year, DateTime.Now.Month, 1 );

                    int maxMeses = 6;

                    while (true)
                    {
                        var lst = repository.ObterListaFechamento ( db, 
                                                                    dt.Month.ToString().PadLeft(2, '0'), 
                                                                    dt.Year.ToString(),
                                                                    (long) empresa.i_unique,
                                                                    (long) associadoPrincipal.i_unique );

                        if (lst.Count() == 0 && maxMeses == 6)
                        {
                            dt = dt.AddMonths(-1);
                            continue;
                        }
                        else
                            if (lst.Count() == 0)
                                break;

                        var lstIdParcelas = lst.Select(y => (long)y.fk_parcela).ToList();
                        var parcelas = repository.ObterListaParcela(db, lstIdParcelas);

                        var lstIdLojas = lst.Select(y => (long)y.fk_loja).ToList();
                        var lojas = repository.ObterListaLoja(db, lstIdLojas);

                        var vendas = new List<AssociadoExtratoVenda>();

                        foreach (var venda in lst)
                        {
                            var parc = parcelas.FirstOrDefault(y => y.i_unique == venda.fk_parcela);

                            if (parc == null)
                                continue;

                            var loja = lojas.FirstOrDefault(y => y.i_unique == venda.fk_loja);

                            if (loja == null)
                                continue;

                            vendas.Add(new AssociadoExtratoVenda
                            {
                                dtVenda = Convert.ToDateTime(venda.dt_compra).ToString("dd/MM/yyyy HH:mm"),
                                nsu = parc.nu_nsu.ToString(),
                                estab = loja.st_nome,
                                parcela = parc.nu_indice.ToString() + " / " + parc.nu_tot_parcelas.ToString(),
                                valor = mon.setMoneyFormat ((long)parc.vr_valor),
                            });
                        }

                        dto.faturas.Add(new AssociadoExtratoFechadoSintetico
                        {
                            mesAno = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year,
                            valorTotal = "R$ " + mon.setMoneyFormat((long)lst.Sum(y => y.vr_valor)),
                            vendas = vendas
                        });

                        dt = dt.AddMonths(-1);

                        if (--maxMeses == 0)
                            break;
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
