using Entities.Api.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Entities.Api
{
    public class BaseController : ControllerBase
    {
        public IConfiguration configuration;
        
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
                sessionEnsemble = claims.FirstOrDefault(claim => claim.Type == "sessionEnsemble")?.Value,
            };
        }
    }
}
