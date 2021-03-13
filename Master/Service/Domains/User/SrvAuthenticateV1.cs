using Entities.Api;
using Entities.Api.User;
using Master.Infra.Entity.Database;
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
        IDapperRepository repository;

        public SrvAuthenticateV1 (IDapperRepository _repository) 
        {
            repository = _repository;
        }

        bool ValidadeRequest(DtoLoginInformation dto)
        {
            #region - code - 

            switch (dto.userType)
            {
                case "2":
                    {
                        if (dto.empresa == null)
                        {
                            Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                            return false;
                        }

                        if (dto.matricula == null)
                        {
                            Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                            return false;
                        }

                        if (dto.venc == null)
                        {
                            Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                            return false;
                        }

                        if (dto.codAcesso == null)
                        {
                            Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                            return false;
                        }

                        if (dto.senha == null)
                        {
                            Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                            return false;
                        }

                        break;
                    }

                default:
                    {
                        Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                        return false;
                    }
            }

            if (dto._language == null)
            {
                Error = new DtoServiceError { message = getLanguage(dto._language, 1) };
                return false;
            }

            return true;

            #endregion
        }

        public bool Exec ( LocalNetwork network, DtoLoginInformation dto, ref DtoAuthenticatedUser loggedUser ) 
        {
            try
            {
                if (!ValidadeRequest(dto))
                    return false;
                
                using (var db = GetConnection(network))
                {
                    switch (dto.userType)
                    {
                        case "2":
                            {
                                #region - code - 

                                var t_emp = repository.GetEmpresaNum(db, Convert.ToInt64(dto.empresa));

                                if (t_emp == null)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                if (t_emp.bBlocked == true)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                var t_associado = repository.GetCartao(db, t_emp.id, Convert.ToInt64(dto.matricula), 1);

                                if (t_associado == null)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                var descript_senha = DESdeCript(t_associado.stSenha);

                                if (descript_senha != dto.senha)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                if (t_associado.nuStatus != CartaoStatus.Habilitado)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                if (t_associado.stVenctoCartao != dto.venc)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                var codAcesso = this.ObterCodigoAcesso ( t_emp.nuEmpresa, 
                                                                         t_associado.nuMatricula, 
                                                                         1, 
                                                                         t_associado.nuViaCartao, 
                                                                         t_associado.stCpf );

                                if (codAcesso != dto.codAcesso)
                                {
                                    Error = new DtoServiceError
                                    {
                                        message = getLanguage(dto._language, 1),
                                        debugInfo = ""
                                    };

                                    return false;
                                }

                                loggedUser._id = t_associado.id.ToString();
                                loggedUser._type = dto.userType;
                                loggedUser.email = "";
                                loggedUser.terminal = "";
                                loggedUser.nome = t_associado.stNome;                                
                                loggedUser.empresa = t_emp.nuEmpresa.ToString();
                                loggedUser.matricula = t_associado.nuMatricula.ToString();

                                #endregion

                                break;
                            }
                    }
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
