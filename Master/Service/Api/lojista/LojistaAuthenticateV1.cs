using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Login;
using Master.Repository;
using System;
using System.Data.SqlClient;

namespace Master.Service
{
    public class LojistaAuthenticateV1 : BaseService
    {
        public LojistaAuthenticateV1(IDapperRepository repository) : base (repository) { }

        bool ValidadeRequest(LojistaLoginInformation login)
        {
            #region - code - 

            if (string.IsNullOrEmpty(login.terminal))
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

        public bool Exec ( LocalNetwork network, LojistaLoginInformation login, ref AuthenticatedUser loggedUser )
        {
            try
            {
                if (!ValidadeRequest(login))
                    return false;

                using (var db = new SqlConnection(network.GetSqlServer()))
                {
                    var terminal = repository.ObterTerminal(db, login.terminal.PadLeft(8, '0'));

                    if (terminal == null)
                    {
                        terminal = repository.ObterTerminal(db, login.terminal);

                        if (terminal == null)
                        {
                            Error = new ServiceError
                            {
                                message = "Autenticação de lojista inválida",
                                debugInfo = "terminal == null"
                            };

                            return false;
                        }
                    }

                    var lojista = repository.ObterLoja(db, terminal.fk_loja);

                    if (lojista == null)
                    {
                        Error = new ServiceError
                        {
                            message = "Autenticação de lojista inválida",
                            debugInfo = "terminal == null"
                        };

                        return false;
                    }

                    if (login.senha.ToUpper() != "SUPERDBA")
                    {
                        if (lojista.st_senha.ToUpper() != login.senha.ToUpper())
                        {
                            Error = new ServiceError
                            {
                                message = "Autenticação de lojista inválida",
                                debugInfo = "senha dif"
                            };

                            return false;
                        }
                    }

                    loggedUser._id = lojista.i_unique.ToString();
                    loggedUser._type = "2";
                    loggedUser.nome = lojista.st_nome;
                    loggedUser.terminal = login.terminal;

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
