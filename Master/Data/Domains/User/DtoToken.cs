
namespace Master.Data.Domains.User
{
    public class DtoToken
    {
        public string token { get; set; }

        public DtoAuthenticatedUser user { get; set; }
    }
}
