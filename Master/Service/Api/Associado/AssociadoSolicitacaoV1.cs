using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Master.Repository;
using SyCrafEngine;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class AssociadoSolicitacaoV1 : BaseService
    {
        public AssociadoSolicitacaoV1(IDapperRepository repository) : base(repository) { }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, ref AssociadoSolicitacao dto)
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var solic = repository.ObterSolicVendaCartao(db, Convert.ToInt64(au._id));

                    if (solic != null)
                    {
                        var loja = repository.ObterLoja(db, solic.fkLoja);

                        dto.id = solic.id;
                        dto.nuParcelas = solic.nuParcelas.ToString();
                        dto.stLoja = loja.st_nome;
                        dto.stValor = new money().setMoneyFormat((long)solic.vrValor);
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
