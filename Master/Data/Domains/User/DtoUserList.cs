using System.Collections.Generic;

namespace Entities.Api.User
{
    public class DtoUserList : DtoBase
    {
        public long total { get; set; }
        public List<Master.Infra.Entity.Database.User> results { get; set; }
    }
}
