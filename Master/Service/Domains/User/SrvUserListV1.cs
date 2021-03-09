using Entities.Api;
using Entities.Api.User;
using Master.Repository;
using System;
using System.Collections.Generic;

namespace Master.Service
{
    public class SrvML_UserList : SrvBaseService
    {
        #region - multi-language - 

        List<List<string>> multi_language = new List<List<string>>
        {
            new List<string> // english
            {
                "Ops, something went wrong", /* 0 */
                "Invalid user authentication", /* 1 */
                "User already registered", /* 2 */
                "Email not found", /* 3 */
                "Invalid Token", /* 4 */
            },
            new List<string> // spanish
            {
                "Ops, algo salió mal", /* 0 */
                "Autenticación de usuario no válida", /* 1 */
                "Usuario ya registrado", /* 2 */
                "El correo electrónico no encontrado", /* 3 */
                "Simbolo no valido", /* 4 */
            },
            new List<string> // pt-br
            {
                "Ops, algo deu errado", /* 0 */
                "Autenticação inválida", /* 1 */
                "Usuário já registrado", /* 2 */
                "Email não encontrado", /* 3 */
                "Token inválido", /* 4 */
            },
        };

        public string getLanguage(string indexLanguage, int messageIndex)
        {
            return multi_language[Convert.ToInt32(indexLanguage)][messageIndex];
        }

        #endregion
    }

    public class SrvUserListV1 : SrvML_UserList
    {
        IDapperUserRepository repository;

        public SrvUserListV1(IDapperUserRepository _repository) 
        {
            repository = _repository;
        }

        public bool Exec ( LocalNetwork network, DtoAuthenticatedUser au, string pesquisa, ref DtoUserList dto )
        {
            try
            {
                using (var db = GetConnection(network))
                {
                    var lstClient = repository.GetUsers(db, "%" + pesquisa + "%");

                    dto.total = lstClient.Count;
                    dto.results = lstClient;
                    
                    foreach (var item in dto.results)
                    {
                        // extra processing
                    }
                }
    
                return true;
            }
            catch (Exception ex)
            {
                Error = new DtoServiceError
                {
                    message = getLanguage("0", 0),
                    debugInfo = ex.ToString()
                };

                return false;
            }
        }        
    }
}
