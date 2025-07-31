using Microsoft.IdentityModel.Tokens;
using Nubetico.Shared.Dto.Core;
using Nubetico.Shared.Enums.Core;
using Nubetico.WebAPI.Application.Modules.Core.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nubetico.WebAPI.Application.Utils
{
    public class JwtHandlerService
    {
        private readonly IConfiguration _config;

        public JwtHandlerService(IConfiguration config)
        {
            _config = config;
        }

        public JwtDataDto UserJwtSigner(UserJwtRequestModel userJwtRequestModel)
        {
            int minutesValid = int.TryParse(_config["JwtConfig:MinutesValid"], out var duration) ? duration : 0;
            var tokenHandler = new JwtSecurityTokenHandler();
            string key = _config["JwtConfig:Key"] ?? string.Empty;
            var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, userJwtRequestModel.Username),
                new Claim(ClaimTypes.Email, userJwtRequestModel.Email),
                new Claim("name", userJwtRequestModel.Nombre),
                new Claim("id", userJwtRequestModel.Id),
                new Claim("tenant-id", userJwtRequestModel.TenantGuid), // Agregar a la firma del Jwt el Guid del tenant
            };

            if (userJwtRequestModel.EntidadContacto != null)
            {
                var tipo = ((int)(userJwtRequestModel.EntidadContacto.Tipo ?? TypeContactUserEnum.None)).ToString();
                claims.Add(new Claim("type-contact", tipo));
                claims.Add(new Claim("entity-contact-id", userJwtRequestModel.EntidadContacto.Id.ToString()));
                claims.Add(new Claim("entity-contact-name", userJwtRequestModel.EntidadContacto.Nombre));
            }
            else
            {
                claims.Add(new Claim("type-contact", ((int)TypeContactUserEnum.None).ToString()));
            }

            var tokenDescritor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(minutesValid),
                SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256Signature),
                Issuer = _config["JwtConfig:Issuer"],
                Audience = _config["JwtConfig:Audience"]
            };

            var token = tokenHandler.CreateToken(tokenDescritor);
            var tokenString = tokenHandler.WriteToken(token);
            return new JwtDataDto { Token = tokenString, Expiracion = tokenDescritor.Expires };
        }
    }
}
