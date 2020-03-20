using Entities.Api;
using Entities.Api.Lojista;
using Master.Repository;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class SolicitaVendaCartaoV1 : BaseService
    {
        public SolicitaVendaCartaoV1(IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(ReqSolicitacaoVendaCartao req)
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

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, ReqSolicitacaoVendaCartao req, ref string nomeCartao )
        {
            try
            {
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    if (!ValidadeRequest(req))
                        return false;

                    req.empresa = req.empresa.PadLeft(6, '0');
                    req.matricula = req.matricula.PadLeft(6, '0');

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

                    nomeCartao = dadosProprietario.st_nome;

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
