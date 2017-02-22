using JwtAuthForWebAPI;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace App.Web.Controllers
{
    public class AutenticacaoController : ApiControllerBase
    {
        [Authorize]
        public string Get(string id)
        {
            return "abc";
        }

        [HttpPost]
        public string PostSignIn(dynamic user)
        {
            var builder = new SecurityTokenBuilder();

            var credentials = new SigningCredentials(
                new InMemorySymmetricSecurityKey(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11,12,13,14,15,16,17,18,19,20,21,22,23,24,25 }),
                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                "http://www.w3.org/2001/04/xmlenc#sha256");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new List<Claim>()
                            {
                                new Claim(ClaimTypes.Name, "bsmith"), 
                                new Claim(ClaimTypes.GivenName, "Bob"),
                                new Claim(ClaimTypes.Surname, "Smith"),
                                new Claim(ClaimTypes.Role, "Customer Service")
                            }),
                TokenIssuerName = "corp",
                AppliesToAddress = "http://www.novopipeline.com.br",
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
