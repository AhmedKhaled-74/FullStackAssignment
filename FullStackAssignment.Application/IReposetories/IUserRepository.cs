using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.IReposetories
{
    public interface IUserRepository
    {
        Task RegisterUserAsync(User user);
        Task<User?> LoginUserAsync(string usernameOrEmail, string password);
        Task UpdateUserAsync(User user);
        Task<User?> FindUserByEmailAsync(string email);
        Task<User?> FindUserByUserNameAsync(string username);
    }
}
