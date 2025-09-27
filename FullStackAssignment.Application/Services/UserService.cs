using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.IServices;
using FullStackAssignment.Application.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtServiceReposetory _jwtServiceReposetory;

        public UserService(IUserRepository userRepository , IJwtServiceReposetory jwtServiceReposetory)
        {
            _userRepository = userRepository;
            _jwtServiceReposetory = jwtServiceReposetory;
        }

        public async Task<AuthenticationResponse> RegisterAsync(RegisterDTO? registerDto)
        {
            if (registerDto == null) throw new ArgumentNullException(nameof(registerDto));
            var exsistUser = await _userRepository.FindUserByUserNameAsync(registerDto.UserName!);
            if (exsistUser != null) throw new ArgumentException(nameof(registerDto.UserName) , "Username is in used");
            var exsistUserFromEmail = await _userRepository.FindUserByEmailAsync(registerDto.Email!);
            if (exsistUserFromEmail != null) throw new ArgumentException(nameof(registerDto.Email), "Email Address is in used");

            var user = registerDto.ToUserEntity();
            await _userRepository.RegisterUserAsync(user);

            var authenticationResponse = _jwtServiceReposetory.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpirationDate;
            await _userRepository.UpdateUserAsync(user);

            return authenticationResponse;

        }
        public async Task<AuthenticationResponse> LoginAsync(LoginDTO? loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));
            var user = await _userRepository.LoginUserAsync(loginDto.UserNameOrEmail!,loginDto.Password!);
            if (user == null) throw new ArgumentException("UserName or Password is invalid");

            var authenticationResponse = _jwtServiceReposetory.CreateJwtToken(user);
            user.RefreshToken = authenticationResponse.RefreshToken;
            user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpirationDate;
            user.LastLoginDate = DateTime.UtcNow;
            await _userRepository.UpdateUserAsync(user);

            return authenticationResponse;
        }

        public async Task LogoutAsync(string? token)
        {
            if (token == null)
                throw new ArgumentNullException(nameof(token));

            var principal = _jwtServiceReposetory.GetJwtPrincipal(token);
            if (principal == null)
                throw new ArgumentException("Invalid Jwt access token");

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (email == null)
                throw new ArgumentException("Invalid Jwt access token no email claim");
            var user = await _userRepository.FindUserByEmailAsync(email);

            if (user == null)
                throw new ArgumentException("User not found");
            // Invalidate the refresh token
            user.RefreshToken = null;
            user.RefreshTokenExpiration = null;
            await _userRepository.UpdateUserAsync(user);
        }
        public async Task<bool> FindUserByEmail(string? email)
        {
            if (email == null) throw new ArgumentNullException(nameof(email));
            var user = await _userRepository.FindUserByEmailAsync(email);
            if (user == null) return false;
            return true;
        }

        public async Task<bool> FindUserByUserName(string? username)
        {
            if (username == null) throw new ArgumentNullException(nameof(username));
            var user = await _userRepository.FindUserByUserNameAsync(username);
            if (user == null) return false;
            return true;
        }
    }
}
