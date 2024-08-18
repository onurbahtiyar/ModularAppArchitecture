using Core.Entities.Concrete;
using Core.Entities.Extensions;
using Core.Utilities.Security.Encryption;
using Enigma;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Core.Utilities.Security.JWT
{
    public class JwtHelper : ITokenHelper
    {
        private readonly TokenOptions _tokenOptions;
        private DateTime _accessTokenExpiration;
        private readonly Processor processor;

        public JwtHelper(IConfiguration configuration, Processor processor)
        {
            this.processor = processor;

            var securityKey = configuration["JwtSettings:SecurityKey"];
            _tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>();

            using (Aes aes = Aes.Create())
            {
                _tokenOptions.SecurityKey = processor.DecryptorSymmetric(securityKey, aes);
            }
        }

        public AccessToken CreateToken(TokenUser user)
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signinCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, signinCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new AccessToken
            {
                Token = token,
                Expiration = _accessTokenExpiration,
                User = user,
                IsSuccessful = true
            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, TokenUser user, SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user),
                signingCredentials: signingCredentials
                );
            return jwt;
        }

        private IEnumerable<Claim> SetClaims(TokenUser user)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentitfier(user.Username);
            claims.AddEmail(user.Email);
            claims.AddName(user.Guid.ToString());
            claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return claims;
        }
    }
}