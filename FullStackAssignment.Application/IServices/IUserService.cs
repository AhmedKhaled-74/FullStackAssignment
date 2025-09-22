using FullStackAssignment.Application.DTOs.UserDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IServices
{
    public interface IUserService
    {
        Task<AuthenticationResponse> RegisterAsync(RegisterDTO? register);
        Task<AuthenticationResponse> LoginAsync(LoginDTO? register);
        Task LogoutAsync(string? token);
        Task<bool> FindUserByEmail(string? email);
        Task<bool> FindUserByUserName(string? username);
    }
}
