using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Entities.Enums;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class AssociadoLimitesV1 : BaseService
    {
        public AssociadoLimitesV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoLimites dto)
        {
            try
            {
                using (var db = new SqlConnection(network.sqlServer))
                {
                    var associadoPrincipal = repository.ObterCartao(db, au._id);
                    var empresa = repository.ObterEmpresa(db, associadoPrincipal.st_empresa);

                    var mon = new money();

                    long    dispMensal = 0,
                            dispTotal = 0, 
                            vrUtilizadoAtual = 0;

                    new BuscaSaldo().SaldoCartao ( repository, 
                                                   db,
                                                   associadoPrincipal, 
                                                   ref dispMensal, 
                                                   ref dispTotal, 
                                                   ref vrUtilizadoAtual );

                    long varTotMensal = (long)associadoPrincipal.vr_limiteMensal + (long)associadoPrincipal.vr_extraCota;

                    dto.limiteCartao = "R$ " + mon.setMoneyFormat(varTotMensal);
                    dto.parcelas = empresa.nu_parcelas.ToString();
                    dto.limiteMensalDisp = "R$ " + mon.setMoneyFormat(dispMensal);
                    dto.cotaExtra = "R$ " + mon.setMoneyFormat((long)associadoPrincipal.vr_extraCota);
                    dto.melhorDia = empresa.nu_diaFech.ToString().PadLeft(2, '0');
                    dto.mensalUtilizado = "R$ " + mon.setMoneyFormat(vrUtilizadoAtual);

                    var dt = DateTime.Now;

                    if (DateTime.Now.Day >= empresa.nu_diaFech)
                        dt = dt.AddMonths(1);

                    dto.mesVigente = new EnumMonth().Get(dt.Month).stName + " / " + dt.Year;

                    dto.pct = (dispMensal * 100 / varTotMensal ).ToString();
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
