using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Lojista;
using Master.Repository;
using Newtonsoft.Json;
using RestSharp;
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
                    var solic = repository.ObterSolicitacaoId(db, Convert.ToInt64(req.id));

                    var log_trans = repository.ObterLogTransacao(db, (long) solic.fkLogTrans);                    

                    // --------------------------------
                    // executa cancelamento de venda
                    // --------------------------------

                    {
                        var serviceClient = new RestClient(network.GetConveyNetAPI());
                        var serviceRequest = new RestRequest("api/VendaCancServerISO", Method.PUT);

                        serviceRequest.AddJsonBody(new { st_nsu = log_trans.nu_nsu });
                    
                        var response = serviceClient.Execute(serviceRequest);
                        var retornoVenda = JsonConvert.DeserializeObject<VendaCancIsoOutputDTO>(response.Content);

                        if (string.IsNullOrEmpty(retornoVenda.st_codResp))
                        {
                            Error = new ServiceError
                            {
                                message = "Ops, serviço indisponível",
                                debugInfo = "string.IsNullOrEmpty(retornoVenda.st_codResp)"
                            };

                            return false;
                        }
                    }

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
