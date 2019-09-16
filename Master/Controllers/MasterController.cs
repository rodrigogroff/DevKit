using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Entities.Api.Configuration;
using Master;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Api.Master.Controllers
{
    [Authorize]
    [ApiController]
    public partial class MasterController : ControllerBase
    {
        public LocalNetwork network;
        public Features features;

        public MasterController(IOptions<Features> _feature, IOptions<LocalNetwork> _network)
        {
            this.features = _feature.Value;
            this.network = _network.Value;
        }

        [NonAction]
        protected AuthenticatedUser GetCurrentAuthenticatedUser()
        {
            var handler = new JwtSecurityTokenHandler();
            var authHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(authHeader))
                return null;

            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var claims = tokenS.Claims;

            return new AuthenticatedUser
            {
                cpf = claims.FirstOrDefault(claim => claim.Type == "cpf")?.Value,
                name = claims.FirstOrDefault(claim => claim.Type == "name")?.Value,
                email = claims.FirstOrDefault(claim => claim.Type == "email")?.Value,
                cnpj = claims.FirstOrDefault(claim => claim.Type == "cnpj")?.Value,
                sessionEnsemble = claims.FirstOrDefault(claim => claim.Type == "sessionEnsemble")?.Value,                
            };
        }

        [NonAction]
        public string ComposeTokenForSession(AuthenticatedUser au)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(LocalNetwork.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("cpf", au.cpf),
                    new Claim("name", au.name),
                    new Claim("email", au.email),
                    new Claim("cnpj", au.cnpj),
                    new Claim("sessionEnsemble", au.sessionEnsemble),
                }),
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = new SigningCredentials (   new SymmetricSecurityKey(key), 
                                                                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
