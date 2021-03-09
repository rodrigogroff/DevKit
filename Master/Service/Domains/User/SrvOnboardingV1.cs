using Entities.Api;
using Entities.Api.User;
using Master.Infra.Entity.Database;
using Master.Repository;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Master.Service
{
    public class SrvML_Onboarding : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Invalid social ID", /* 1 */
                "Invalid email", /* 2 */
                "Invalid name", /* 3 */
                "Invalid password", /* 4 */
                "User already registered (email)", /* 5 */
                "User already registered (social ID)", /* 6 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                 "ID social no válido", /* 1 */
                 "Correo electrónico no válido", /* 2 */
                 "Nombre no válido", /* 3 */
                 "Contraseña no válida", /* 4 */
                 "Usuario ya registrado (correo electrónico)", /* 5 */
                 "Usuario ya registrado (ID social)", /* 6 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                 "ID social inválido", /* 1 */
                 "Email inválido", /* 2 */
                 "Nome inválido", /* 3 */
                 "Senha inválida", /* 4 */
                 "Usuário já cadastrado (email)", /* 5 */
                 "Usuário já cadastrado (ID social)", /* 6 */
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

    public class SrvOnboardingV1 : SrvML_Onboarding
    {
        IDapperUserRepository repository;

        public SrvOnboardingV1(IDapperUserRepository _repository)
        {
            repository = _repository;
        }

        bool ValidadeRequest(DtoOnboarding dto)
        {
            #region - code - 

            if (string.IsNullOrEmpty(dto.sID))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            if (string.IsNullOrEmpty(dto.sEmail))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 2) };
                return false;
            }

            if (string.IsNullOrEmpty(dto.sName))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 3) };
                return false;
            }

            if (string.IsNullOrEmpty(dto.sPass))
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 4) };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec(LocalNetwork network, DtoOnboarding dto)
        {
            if (!ValidadeRequest(dto))
                return false;

            try
            {
                dto.sEmail = dto.sEmail.ToLower().Trim();
                dto.sID = dto.sID.ToLower().Trim();
                dto.sName = dto.sName.ToLower().Trim();

                using (var db = GetConnection(network))
                {
#if MASSLOAD
                    User user = null;
#else

                    User user = repository.GetUserByEmail(db, dto.sEmail.ToLower().Trim());

                    if (user != null)
                    {
                        if (user.bTokenized == true)
                        {
                            Error = new DtoServiceError
                            {
                                message = getLanguage(dto._language, 5),
                                debugInfo = "[1]"
                            };

                            return false;
                        }
                    }

                    user = repository.GetUserBySocial(db, dto.sID);

                    if (user != null)
                    {
                        if (user.bTokenized == true)
                        {
                            Error = new DtoServiceError
                            {
                                message = getLanguage(dto._language, 6),
                                debugInfo = "[2]"
                            };

                            return false;
                        }
                    }

                    #endif

#if MASSLOAD
                    var token = "1234";
#else
                    var token = "";
                    var rnd = new Random();
                    for (int i = 0; i < 4; i++)
                    {
                        token += rnd.Next(0, 9);
                        Thread.Sleep(33);
                    }
#endif

                    if (user == null)
                    {
                        user = new User
                        {
                            bAdmin = false,
                            bActive = true,
                            bTokenized = false,
                            dtCreation = DateTime.Now,
                            dtLastLogin = null,
                            stEmail = dto.sEmail,
                            stName = dto.sName,
                            stPassword = dto.sPass,
                            stSocialID = dto.sID,
                            stToken = token,
                            dtTokenExpires = DateTime.Now.AddSeconds(3 * 60),
                        };

#if MASSLOAD                        
                        user.bTokenized = true;
                        _sendEmail = false;                                                
#endif

                        user.compile();

                        repository.InsertUser(db, user);
                    }
                    else
                    {
                        user.bTokenized = false;
                        user.stToken = token;
                        user.dtTokenExpires = DateTime.Now.AddSeconds(3 * 60);

                        repository.UpdateUser(db, user);
                    }

                    SendEmail(dto.sEmail.ToLower(), "Onboarding", "Token: " + token);
                }

                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage(dto._language, 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }
    }
}
