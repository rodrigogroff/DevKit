
namespace Entities.Api.User
{
    public class DtoAuthenticatedUser : DtoBase
    {
        public string _id { get; set; }
        public string email { get; set; }
        public string nome { get; set; }
        public string _type { get; set; }
        public string empresa { get; set; }
        public string matricula { get; set; }
        public string terminal { get; set; }
    }
}
