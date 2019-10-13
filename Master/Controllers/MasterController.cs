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
                _id = claims.FirstOrDefault(claim => claim.Type == "_id")?.Value,
                _type = claims.FirstOrDefault(claim => claim.Type == "_type")?.Value,
                terminal = claims.FirstOrDefault(claim => claim.Type == "terminal")?.Value,
                nome = claims.FirstOrDefault(claim => claim.Type == "nome")?.Value,
                empresa = claims.FirstOrDefault(claim => claim.Type == "empresa")?.Value,
                matricula = claims.FirstOrDefault(claim => claim.Type == "matricula")?.Value,
            };
        }

        [NonAction]
        public string ComposeTokenForSession(AuthenticatedUser au)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(LocalNetwork.Secret);

            if (au.nome == null) au.nome = "";
            if (au.terminal == null) au.terminal = "";
            if (au.empresa == null) au.empresa = "";
            if (au.matricula == null) au.matricula = "";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("_id", au._id),
                    new Claim("_type", au._type),
                    new Claim("name", au.nome),
                    new Claim("terminal", au.terminal),
                    new Claim("empresa", au.empresa),
                    new Claim("matricula", au.matricula),
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
