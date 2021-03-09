using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Master.Service
{
    public class SrvML_ResendToken : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Social ID not found", /* 1 */               
                "Onboarding", /* 2 */     
                "Token: ", /* 3 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "ID social no encontrado", /* 1 */
                "Inducción", /* 2 */
                "Token: ", /* 3 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "CPF não encontrado ", /* 1 */
                "Cadastro", /* 2 */
                "Token: ", /* 3 */
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

    public class SrvResendTokenV1 : SrvML_ResendToken
    {
        IDapperUserRepository repository;

        public SrvResendTokenV1(IDapperUserRepository _repository)
        {
            repository = _repository;
        }

        public bool Exec(LocalNetwork network, DtoResendToken obj)
        {
            try
            {
                using (var db = GetConnection(network))
                {
                    var user = repository.GetUserBySocial(db, obj.sID);

                    if (user == null)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(obj._language, 1),
                            debugInfo = "[1]"
                        };

                        return false;
                    }

                    var token = "";
                    var rnd = new Random();
                    for (int i = 0; i < 4; i++)
                    {
                        token += rnd.Next(0, 9);
                        Thread.Sleep(1);
                    }

                    user.stToken = token;

                    repository.UpdateUser(db, user);

                    SendEmail(user.stEmail, getLanguage(obj._language, 2), getLanguage(obj._language, 3) + token);
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
