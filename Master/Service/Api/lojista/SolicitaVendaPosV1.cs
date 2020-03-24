using Entities.Api;
using Entities.Api.Associado;
using Entities.Api.Login;
using Entities.Api.Lojista;
using Entities.Database;
using Master.Repository;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class SolicitaVendaPosV1 : BaseService
    {
        public SolicitaVendaPosV1(IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(ReqSolicitacaoVendaPOS req)
        {
            #region - code - 

            if (string.IsNullOrEmpty(req.empresa))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.matricula))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.codAcesso))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.venc))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.valor))
            {
                Error = new ServiceError { message = "Valor inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.parcelas))
            {
                Error = new ServiceError { message = "Num. Parcelas inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(req.senha) || req.senha.Length < 4)
            {
                Error = new ServiceError { message = "Senha inválida" };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, long fkTerminal, ReqSolicitacaoVendaPOS req )
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    if (!ValidadeRequest(req))
                        return false;

                    req.empresa = req.empresa.PadLeft(6, '0');
                    req.matricula = req.matricula.PadLeft(6, '0');
                    req.valor = req.valor.Replace(",", "").Replace(".", "");

                    var terminal = repository.ObterTerminal(db, fkTerminal);

                    var associadoPrincipal = repository.ObterCartao(db, req.empresa, req.matricula, "01");

                    if (associadoPrincipal == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal == null"
                        };

                        return false;
                    }

                    if (associadoPrincipal.st_venctoCartao != req.venc)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal.st_venctoCartao != login.venc"
                        };

                        return false;
                    }

                    var tEmp = repository.ObterEmpresa(db, req.empresa);

                    if (tEmp == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "tEmp == null"
                        };

                        return false;
                    }

                    var dadosProprietario = repository.ObterProprietario(db, (int)associadoPrincipal.fk_dadosProprietario);

                    if (dadosProprietario == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    var codAcessoCalc = new CodigoAcesso().Obter(req.empresa,
                                                                   req.matricula,
                                                                   associadoPrincipal.st_titularidade,
                                                                   associadoPrincipal.nu_viaCartao,
                                                                   dadosProprietario.st_cpf);

                    if (codAcessoCalc != req.codAcesso)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    if (terminal == null)
                    {
                        Error = new ServiceError
                        {
                            message = _defaultError,
                            debugInfo = "terminal == null"
                        };

                        return false;
                    }

                    var nu_nsu = "0";

                    // ----------------------------
                    // executa venda
                    // ----------------------------

                    {
                        var serviceClient = new RestClient(network.GetConveyNetAPI());
                        var serviceRequest = new RestRequest("api/VendaServerMobile", Method.PUT);

                        var _parcelas = Convert.ToInt32(req.parcelas);
                        var _valor = Convert.ToInt32(req.valor);

                        var valores = "";

                        if (_parcelas == 1)
                            valores = req.valor.PadLeft(12, '0');
                        else
                            valores = req.parcelas_str;

                        var dadosVenda = new VendaIsoInputDTO
                        {
                            st_empresa = req.empresa,
                            st_matricula = req.matricula,
                            st_senha = DESCript(req.senha),
                            st_terminal = terminal.nu_terminal,
                            st_titularidade = associadoPrincipal.st_titularidade,
                            nu_parcelas = req.parcelas,
                            st_valores = valores,
                            vr_valor = req.valor,
                        };

                        serviceRequest.AddJsonBody(dadosVenda);

                        var response = serviceClient.Execute(serviceRequest);
                        var retornoVenda = JsonConvert.DeserializeObject<Entities.Api.Associado.VendaIsoOutputDTO>(response.Content);

                        if (retornoVenda == null)
                        {
                            Error = new ServiceError
                            {
                                message = _defaultError,
                                data = "",
                                debugInfo = "retornoVenda == null"
                            };

                            // -----------------------------
                            // salvar autorização nok
                            // -----------------------------

                            repository.InserirSolicitacaoVenda(db, new SolicitacaoVenda
                            {
                                dtSolic = DateTime.Now.AddSeconds(-1),
                                dtConf = DateTime.Now,
                                tgAberto = false,
                                fkCartao = (long)associadoPrincipal.i_unique,
                                fkLoja = terminal.fk_loja,
                                fkTerminal = fkTerminal,
                                fkLogTrans = null,
                                stErro = "Falha comunicação servidor",
                                vrValor = Convert.ToInt32(req.valor.Replace(".", "").Replace(",", "")),
                                nuParcelas = Convert.ToInt32(req.parcelas),
                            });

                            return false;
                        }

                        if (!retornoVenda.st_codResp.StartsWith("00"))
                        {
                            Error = new ServiceError
                            {
                                message = retornoVenda.st_msg,
                                debugInfo = "retornoVenda == null"
                            };

                            // -----------------------------
                            // salvar autorização nok
                            // -----------------------------
                            
                            repository.InserirSolicitacaoVenda(db, new SolicitacaoVenda
                            {
                                dtSolic = DateTime.Now.AddSeconds(-1),
                                dtConf = DateTime.Now,
                                tgAberto = false,
                                fkCartao = (long)associadoPrincipal.i_unique,
                                fkLoja = terminal.fk_loja,                                
                                fkTerminal = fkTerminal,
                                fkLogTrans = null,
                                stErro = retornoVenda.st_msg,
                                vrValor = Convert.ToInt32(req.valor.Replace(".", "").Replace(",", "")),
                                nuParcelas = Convert.ToInt32(req.parcelas),
                            });
                            
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

                    // -----------------------------
                    // salvar autorização ok
                    // -----------------------------
                    
                    repository.InserirSolicitacaoVenda(db, new SolicitacaoVenda
                    {
                        dtSolic = DateTime.Now.AddSeconds(-1),
                        dtConf = DateTime.Now,
                        tgAberto = false,
                        fkCartao = (long)associadoPrincipal.i_unique,
                        fkLoja = terminal.fk_loja,
                        fkTerminal = fkTerminal,
                        fkLogTrans = (long?)logTrans.i_unique,
                        stErro = "",
                        vrValor = Convert.ToInt32(req.valor.Replace(".", "").Replace(",", "")),
                        nuParcelas = Convert.ToInt32(req.parcelas),
                    });

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
