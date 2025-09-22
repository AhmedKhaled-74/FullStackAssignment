using FullStackAssignment.Application.IReposetories;
using FullStackAssignment.Application.Mappers;
using FullStackAssignment.Domain.Entites;
using FullStackAssignment.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Infrastructure.Reposetories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task RegisterUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        public async Task<User?> LoginUserAsync(string usernameOrEmail, string password)
        {
            var exsistUser = await _context.Users
                .FirstOrDefaultAsync(u => (u.UserName == usernameOrEmail || u.EmailAddress == usernameOrEmail));
            if (exsistUser != null)
            {
                var checkpass = PasswordHelper.VerifyPassword(password, exsistUser.HashedPassword);
                if(!checkpass) return null;
            }
            return exsistUser;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);  
            await _context.SaveChangesAsync(); 
        }
        public async Task<User?> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress.ToLower() == email.ToLower());
        }

        public async Task<User?> FindUserByUserNameAsync(string username)
        {
            return await _context.Users.FindAsync(username);
        }
    }
}
