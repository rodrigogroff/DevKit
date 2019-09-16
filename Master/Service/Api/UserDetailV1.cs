using Entities.Api.Configuration;
using Entities.Database;

namespace Master.Service
{
    public class UserDetailV1 : BaseService
    {
        public bool Exec(LocalNetwork network, AuthenticatedUser au, string cpf, ref User userInfo)
        {
            return true;
        }
    }
}
