using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Domain.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace FullStackAssignment.Infrastructure.Reposetories
{
    public class JwtServiceRepository : IJwtServiceReposetory
    {
        private readonly IConfiguration _configuration;
        public JwtServiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public AuthenticationResponse CreateJwtToken(User user)
        {
            var expireValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
            var expireRefrechValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:Refresh_EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[] {
                new Claim( JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                             new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                             ClaimValueTypes.Integer64),
                new Claim( ClaimTypes.NameIdentifier, user.EmailAddress), //optional unique email msh bytkrr unique in table
                new Claim( ClaimTypes.Name, user.UserName!),
                new Claim( ClaimTypes.Email, user.EmailAddress),
            };

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expireValue,
                signingCredentials: signingCredentials
                );
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.WriteToken(jwtSecurityToken);
            return new AuthenticationResponse()
            {
                Token = token,
                UserName = user.UserName,
                Expiration = expireValue,
                RefreshToken = GenerateRefreshToken(),
                RefreshTokenExpirationDate = expireRefrechValue
            };
        }
        public ClaimsPrincipal? GetJwtPrincipal(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                ValidateLifetime = false,
            };
            JwtSecurityTokenHandler jwtTokenHandler = new JwtSecurityTokenHandler();
            var principal = jwtTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken? securityToken);

            if (principal == null || securityToken is not JwtSecurityToken jwtSecurity
                || !jwtSecurity.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                  StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        public AuthenticationResponse CreateAccessTokenOnly(User user, string existingRefreshToken, DateTime? existingRefreshExpiry)
        {
            var expireValue = DateTime.UtcNow
               .AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

            Claim[] claims = new Claim[] {
                new Claim( JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim( JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                             new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                             ClaimValueTypes.Integer64),
                new Claim( ClaimTypes.NameIdentifier, user.EmailAddress), //optional unique email msh bytkrr unique in table
                new Claim( ClaimTypes.Name, user.UserName!),
                new Claim( ClaimTypes.Email, user.EmailAddress),
    };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: expireValue,
                signingCredentials: signingCredentials
            );

            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.WriteToken(jwtSecurityToken);

            return new AuthenticationResponse()
            {
                Token = token,
                UserName = user.UserName,
                Expiration = expireValue,
                RefreshToken = existingRefreshToken,
                RefreshTokenExpirationDate = existingRefreshExpiry
            };
        }

        //refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
