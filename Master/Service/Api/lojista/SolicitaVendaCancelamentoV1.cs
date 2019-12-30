using Entities.Api;
using Entities.Api.Lojista;
using Master.Repository;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Master.Service
{
    public class SolicitaVendaCancelamentoV1 : BaseService
    {
        public SolicitaVendaCancelamentoV1(IDapperRepository repository) : base (repository) { }

        public bool Exec ( LocalNetwork network, long fkTerminal, ReqSolicitacaoVenda req )
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var lst = repository.ObterSolicitacoesLista(db, fkTerminal);

                    var found = false;

                    foreach (var item in lst)
                    {
                        if (item.tgAberto == false)
                            continue;

                        item.tgAberto = false;
                                    
                        repository.AtualizarSolicitacaoVenda(db, item);

                        found = true;
                        break;
                    }

                    if (!found)
                    {
                        Error = new ServiceError
                        {
                            message = _defaultError,
                            debugInfo = "!found"
                        };
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
