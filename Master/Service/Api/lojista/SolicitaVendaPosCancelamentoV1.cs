using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Lojista;
using Master.Repository;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Service
{
    public class SolicitaVendaPosCancelamentoV1 : BaseService
    {
        public SolicitaVendaPosCancelamentoV1(IDapperRepository repository) : base (repository) { }

        public bool Exec ( LocalNetwork network, AuthenticatedUser au, ReqSolicitacaoVendaCancelamento req )
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    
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
