using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_CheckNewSocial : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */                
                "User already registered", /* 1 */                
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "Usuario ya registrado", /* 1 */                
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Usuário já registrado", /* 1 */
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

    public class SrvCheckNewSocial_IDV1 : SrvML_CheckNewSocial
    {
        IDapperUserRepository repository;

        public SrvCheckNewSocial_IDV1(IDapperUserRepository _repository) 
        {
            repository = _repository;
        }

        public bool Exec ( LocalNetwork network, DtoCheckNewSocial_ID obj )
        {
            try
            {
                using (var db = GetConnection(network))
                {
                    var user = repository.GetUserBySocial(db, obj.sID);

                    if (user == null)
                        return true;

                    if (user != null)
                    {
                        if (user.bTokenized == true)
                        {
                            Error = new DtoServiceError
                            {
                                message = getLanguage(obj._language, 1),
                                debugInfo = "[1]"
                            };

                            return false;
                        }
                    }                    
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
