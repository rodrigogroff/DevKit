using Entities.Api;
using Entities.Api.Login;
using Master.Repository;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class SolicitaVendaV1 : BaseService
    {
        public SolicitaVendaV1(IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(SolicitacaoVenda login)
        {
            #region - code - 

            if (string.IsNullOrEmpty(login.empresa))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.matricula))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.codAcesso))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.venc))
            {
                Error = new ServiceError { message = "Cartão inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.valor))
            {
                Error = new ServiceError { message = "Valor inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.parcelas))
            {
                Error = new ServiceError { message = "Num. Parcelas inválido" };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, SolicitacaoVenda login )
        {
            try
            {
                if (!ValidadeRequest(login))
                    return false;

                login.empresa = login.empresa.PadLeft(6, '0');
                login.matricula = login.matricula.PadLeft(6, '0');
                
                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var associadoPrincipal = repository.ObterCartao(db, login.empresa, login.matricula, "01");

                    if (associadoPrincipal == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal == null"
                        };

                        return false;
                    }

                    if (associadoPrincipal.st_venctoCartao != login.venc)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "associadoPrincipal.st_venctoCartao != login.venc"
                        };

                        return false;
                    }

                    var tEmp = repository.ObterEmpresa(db, login.empresa);

                    if (tEmp == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "tEmp == null"
                        };

                        return false;
                    }

                    var dadosProprietario = repository.ObterProprietario(db, (int) associadoPrincipal.fk_dadosProprietario);

                    if (dadosProprietario == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    var codAcessoCalc = new CodigoAcesso().Obter ( login.empresa,
                                                                   login.matricula,
                                                                   associadoPrincipal.st_titularidade,
                                                                   associadoPrincipal.nu_viaCartao,
                                                                   dadosProprietario.st_cpf );

                    if (codAcessoCalc != login.codAcesso)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de cartão inválida",
                            debugInfo = "dadosProprietario == null"
                        };

                        return false;
                    }

                    // -----------------------
                    // salvar no banco!!
                    // -----------------------
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
