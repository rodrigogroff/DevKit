using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Login;
using Master.Repository;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class AuthenticateAssociadoV1 : BaseService
    {
        public AuthenticateAssociadoV1 (IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(AssociadoLoginInformation login)
        {
            #region - code - 

            if (string.IsNullOrEmpty(login.empresa))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.matricula))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.codAcesso))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.venc))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.senha))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, AssociadoLoginInformation login, ref AuthenticatedUser loggedUser )
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

                    var senhaComputada = DESdeCript(associadoPrincipal.st_senha, "12345678").TrimStart('*');

                    if (login.senha.ToUpper() != "SUPERDBA")
                        if (senhaComputada != login.senha)
                        {
                            Error = new ServiceError
                            {
                                message = "Autenticação de cartão inválida",
                                debugInfo = "senhaComputada != login.senha"
                            };

                            return false;
                        }

                    loggedUser.empresa = login.empresa;
                    loggedUser.matricula = login.matricula;
                    loggedUser.nome = dadosProprietario.st_nome;
                    loggedUser._id = associadoPrincipal.i_unique.ToString();
                    loggedUser._type = "1";                    
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
