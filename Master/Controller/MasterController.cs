using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Entities.Api.User;
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
        public bool _doNotSendEmail = false;

        public MasterController(IOptions<LocalNetwork> _network)
        {
            if (_network != null)
                this.network = _network.Value;

        }

        [NonAction]
        protected DtoAuthenticatedUser GetCurrentAuthenticatedUser()
        {
            var handler = new JwtSecurityTokenHandler();
            var authHeader = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(authHeader))
                return null;

            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;
            var claims = tokenS.Claims;

            return new DtoAuthenticatedUser
            {
                _id = claims.FirstOrDefault(claim => claim.Type == "_id")?.Value,
                email = claims.FirstOrDefault(claim => claim.Type == "email")?.Value,
                nome = claims.FirstOrDefault(claim => claim.Type == "login")?.Value,
                _type = claims.FirstOrDefault(claim => claim.Type == "userType")?.Value,
            };
        }

        [NonAction]
        public string ComposeTokenForSession(DtoAuthenticatedUser au)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(LocalNetwork.Secret);

            if (au.email == null) au.email = "";
            if (au.nome == null) au.nome = "";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                /*
                 * email = claims.FirstOrDefault(claim => claim.Type == "email")?.Value,
                nome = claims.FirstOrDefault(claim => claim.Type == "login")?.Value,
                _type = claims.FirstOrDefault(claim => claim.Type == "userType")?.Value,*/

                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("_id", au._id),
                    new Claim("_type", au._type),
                    new Claim("nome", au.nome),
                    new Claim("email", au.email),                    
                }),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                                                SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
