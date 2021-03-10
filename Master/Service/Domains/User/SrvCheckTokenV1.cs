using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_CheckToken : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Email not found", /* 1 */
                "Invalid Token", /* 2 */
                "Token expired!", /* 3 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "El correo electrónico no encontrado", /* 1 */
                "Simbolo no valido", /* 2 */
                "¡Token caducado!", /* 3 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Email não encontrado", /* 1 */
                "Token inválido", /* 2 */
                "Token expirado", /* 3 */
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

    public class SrvCheckTokenV1 : SrvML_CheckToken
    {
        IDapperRepository repository;

        public SrvCheckTokenV1(IDapperRepository _repository) 
        {
            repository = _repository;
        }

        public bool Exec ( LocalNetwork network, DtoCheckToken obj )
        {
            try
            {
                using (var db = GetConnection(network))
                {
                    /*
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

                    if (user.stToken != obj.sToken)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(obj._language, 2),
                            debugInfo = "[1]"
                        };

                        return false;
                    }

                    if (DateTime.Now > user.dtTokenExpires)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(obj._language, 3),
                            debugInfo = "[1]"
                        };

                        return false;
                    }

                    user.bTokenized = true;

                    repository.UpdateUser(db, user);
                    */
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
