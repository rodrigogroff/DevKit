using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_Authenticate : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Invalid user authentication", /* 1 */                
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "Autenticación de usuario no válida", /* 1 */                
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Autenticação inválida", /* 1 */                
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

    public class SrvAuthenticateV1 : SrvML_Authenticate
    {
        IDapperUserRepository repository;

        public SrvAuthenticateV1 (IDapperUserRepository _repository) 
        {
            repository = _repository;
        }

        bool ValidadeRequest(DtoLoginInformation dto)
        {
            if (string.IsNullOrEmpty(dto.email))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (string.IsNullOrEmpty(dto.password))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            return true;
        }

        public bool Exec ( LocalNetwork network, DtoLoginInformation dto, ref DtoAuthenticatedUser loggedUser ) 
        {
            try
            {
                if (!ValidadeRequest(dto))
                    return false;
                
                using (var db = GetConnection(network))
                {
                    dto.email = dto.email.Trim().ToLower();

                    var user = repository.GetUserByEmail(db, dto.email);

                    if (user == null)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(dto._language, 1),
                            debugInfo = "user == null"
                        };

                        return false;
                    }
                    
                    if (user.stPassword != dto.password)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(dto._language, 1),
                            debugInfo = "user.stSenha != login.senha"
                        };

                        return false;
                    }

                    if (user.bActive != true)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(dto._language, 1),
                            debugInfo = "!user.bActive"
                        };

                        return false;
                    }

                    if (user.bTokenized != true)
                    {
                        Error = new DtoServiceError
                        {
                            message = getLanguage(dto._language, 1),
                            debugInfo = "!user.bActive"
                        };

                        return false;
                    }

                    user.dtLastLogin = DateTime.Now;

                    repository.UpdateUser(db, user);

                    loggedUser._id = user.id.ToString();
                    loggedUser.email = user.stEmail;
                    loggedUser.login = user.stName;
                }
    
                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage (dto._language, 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }        
    }
}
