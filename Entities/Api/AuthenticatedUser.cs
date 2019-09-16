
namespace Entities.Api.Configuration
{ 
    public class AuthenticatedUser
    {
        public string _id { get; set; }

        public string cpf { get; set; }

        public string name { get; set; }

        public string email { get; set; }

        public string token { get; set; }

        public string sessionEnsemble { get; set; }

        public string languageOption { get; set; }

        public string cnpj { get; set; }
    }
}
