using Entities.Api.Configuration;
using Entities.Database;
using Master.Repository;

namespace Master.Service
{
    public class UserDetailV1 : BaseService
    {
        public UserDetailV1(IDapperRepository repository) : base(repository)
        {

        }

        public bool Exec(LocalNetwork network, AuthenticatedUser au, string cpf, ref User userInfo)
        {
            return true;
        }
    }
}
