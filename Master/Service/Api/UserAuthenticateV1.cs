using Entities.Api;
using Entities.Api.Configuration;
using Entities.Api.Login;
using Entities.Database;
using System;

namespace Master.Service
{
    public class UserAuthenticateV1 : BaseService
    {
        public bool Exec(   LocalNetwork network,
                            LoginInformation login,
                            ref User userInfo,
                            ref AuthenticatedUser loggedUser)
        {
            try
            {
                //   login.login = OnlyNumbers(login.login);

                if (!ValidadeRequest(login))
                    return false;
    
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

        bool ValidadeRequest(LoginInformation login)
        {
            #region - code - 

            /*
            
            if (string.IsNullOrEmpty(login.login))
            {
                Error = new ServiceError { message = "Login inválido" };
                return false;
            }

            if (login.typeLogin != "1" && login.typeLogin != "2")
            {
                Error = new ServiceError { message = "Tipo de login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.typeLogin))
            {
                Error = new ServiceError { message = "Tipo de login inválido" };
                return false;
            }

            if (string.IsNullOrEmpty(login.passwd))
            {
                Error = new ServiceError { message = "Senha inválida" };
                return false;
            }

            if (login.passwd.Length < 6)
            {
                Error = new ServiceError { message = "Senha inválida" };
                return false;
            }
            */

            return true;

            #endregion
        }
    }
}
