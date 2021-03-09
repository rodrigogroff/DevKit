using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_PasswordRecovery : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Email not found", /* 1 */
                "Recovery", /* 2 */
                "Your password is: ", /* 3 */
                "Invalid email", /* 4 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "El correo electrónico no encontrado", /* 1 */
                "Recuperación", /* 2 */
                "Su contraseña es: ", /* 3 */
                "Correo electrónico no válido", /* 4 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Email não encontrado", /* 1 */
                "Recuperação", /* 2 */
                "Sua senha é: ", /* 3 */
                "Email inválido", /* 4 */
            },
        };

        public string getLanguage(int? indexLanguage, int messageIndex)
        {
            if (indexLanguage == null)
                indexLanguage = 0;

            return multi_language[(int)indexLanguage][messageIndex];
        }

        #endregion
    }

    public class SrvPasswordRecoveryV1 : SrvML_PasswordRecovery
    {
        IDapperUserRepository repository;

        public SrvPasswordRecoveryV1(IDapperUserRepository _repository) 
        {
            repository = _repository;
        }

        public bool Exec ( LocalNetwork network, DtoPasswordRecovery obj )
        {
            try
            {
                if (String.IsNullOrEmpty(obj.email))
                {
                    Error = new DtoServiceError
                    {
                        message = getLanguage(obj._language, 4),
                        debugInfo = "[1]"
                    };

                    return false;
                }

                var email = obj.email.ToLower();

                using (var db = GetConnection(network))
                {
                    var user = repository.GetUserByEmail(db, email);

                    if (user == null)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(obj._language, 1),
                            debugInfo = "[1]"
                        };

                        return false;
                    }

                    SendEmail ( email, getLanguage(obj._language, 2), getLanguage(obj._language, 3) + user.stPassword);
                }
    
                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage(obj._language, 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }        
    }
}
