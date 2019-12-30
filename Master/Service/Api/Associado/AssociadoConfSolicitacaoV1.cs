using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Configuration;
using Master.Repository;
using Newtonsoft.Json;
using RestSharp;
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

                        var cartao = repository.ObterCartao(db, solic.fkCartao.ToString());

                        if (cartao == null)
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                debugInfo = "cartao == null"
                            };

                            return false;
                        }

                        var terminal = repository.ObterTerminal(db, (long)solic.fkTerminal);

                        if (terminal == null)
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                debugInfo = "terminal == null"
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

                        var nu_nsu = "0";

                        // ----------------------------
                        // executa venda
                        // ----------------------------

                        { 
                            var serviceClient = new RestClient(network.GetConveyNetAPI());
                            var serviceRequest = new RestRequest("api/VendaServerWeb", Method.PUT);

                            var valores = "";

                            if (solic.nuParcelas == 1)
                            {
                                valores = solic.vrValor.ToString().PadLeft(12, '0');
                            }
                            else
                            {
                                int _nuParc = (int)solic.nuParcelas;
                                int _vrValor = (int)solic.vrValor;

                                int val = _vrValor / _nuParc;

                                for (int i = 0; i < _nuParc - 1; i++)
                                    valores += val.ToString().PadLeft(12, '0');

                                // ajustar ultima
                                var calc = _vrValor - (val * (_nuParc - 1));
                                valores += calc.ToString().PadLeft(12, '0');                                
                            }

                            var dadosVenda = new VendaIsoInputDTO
                            {
                                st_empresa = cartao.st_empresa,
                                st_matricula = cartao.st_matricula,
                                st_senha = DESCript(dto.stSenha),
                                st_terminal = terminal.nu_terminal,
                                st_titularidade = cartao.st_titularidade,
                                nu_parcelas = solic.nuParcelas.ToString(),
                                st_valores = valores,
                                vr_valor = solic.vrValor.ToString(),
                            };

                            serviceRequest.AddJsonBody(dadosVenda);

                            var response = serviceClient.Execute(serviceRequest);
                            var retornoVenda = JsonConvert.DeserializeObject<VendaIsoOutputDTO>(response.Content);

                            if (retornoVenda == null)
                            {
                                Error = new ServiceError
                                {
                                    message = _defaultError,
                                    data = "",
                                    debugInfo = "retornoVenda == null"
                                };

                                return false;
                            }

                            if (!retornoVenda.st_codResp.StartsWith("00"))
                            {
                                Error = new ServiceError
                                {
                                    message = retornoVenda.st_msg,
                                    debugInfo = "retornoVenda == null"
                                };

                                return false;
                            }

                            nu_nsu = retornoVenda.st_nsuRcb;
                        }

                        // -----------------------
                        // confirma venda
                        // -----------------------

                        {
                            var serviceClient = new RestClient(network.GetConveyNetAPI());
                            var serviceRequest = new RestRequest("api/VendaConfServerISO", Method.PUT);

                            serviceRequest.AddJsonBody(new VendaConfIsoInputDTO
                            {
                                st_nsu = nu_nsu
                            });

                            var response = serviceClient.Execute(serviceRequest);
                        }

                        // -----------------------
                        // buscar nsu
                        // -----------------------

                        var logTrans = repository.ObterLogTransacaoNSU(db, I(nu_nsu));

                        if (logTrans == null)
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                debugInfo = "logTrans == null"
                            };

                            return false;
                        }

                        solic.tgAberto = false;
                        solic.dtConf = DateTime.Now;
                        solic.fkLogTrans = Convert.ToInt64(logTrans.i_unique);

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
