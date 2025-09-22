using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.Services
{
    public class JwtService : IJwtService
    {
        private readonly IJwtServiceReposetory _jwtServiceReposetory;
        private readonly IUserRepository _userRepository;

        public JwtService(IJwtServiceReposetory jwtServiceReposetory , IUserRepository userRepository)
        {
            _jwtServiceReposetory = jwtServiceReposetory;
            _userRepository = userRepository;
        }
        public async Task<AuthenticationResponse> RefreshToken(TokenModel token)
        {
            {
                ArgumentNullException.ThrowIfNull(token);


                var principal = _jwtServiceReposetory.GetJwtPrincipal(token.Token);
                if (principal == null)
                    throw new ArgumentException("Invalid Jwt access token");

                var email = principal.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                    throw new ArgumentException("Invalid Jwt access token no email claim");

                var user = await _userRepository.FindUserByEmailAsync(email);
                if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpiration <= DateTime.UtcNow)
                    throw new ArgumentException("Invalid Jwt access refreshtoken");

                var authRes = _jwtServiceReposetory.CreateAccessTokenOnly(user, user.RefreshToken!, user.RefreshTokenExpiration);

                return authRes;
            }
        }
    }
}
