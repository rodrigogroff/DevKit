using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class AssociadoConfSolicitacaoV1 : BaseService
    {
        public AssociadoConfSolicitacaoV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, AssociadoSolicitacao dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var solic = repository.ObterSolicVendaCartaoId(db, dto.id);

                    if (solic != null)
                    {
                        if (au._id != solic.fkCartao.ToString())
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                debugInfo = "au._id != solic.fkCartao"
                            };

                            return false;
                        }
                        
                        if (solic.tgAberto == false)
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                debugInfo = "solic.tgAberto == false"
                            };

                            return false;
                        }

                        // ----------------------------
                        // executa venda
                        // ----------------------------

                        solic.tgAberto = false;

                        repository.AtualizarSolicitacaoVenda(db, solic);
                    }
                    else
                        dto.id = 0;

                    return true;
                }
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
