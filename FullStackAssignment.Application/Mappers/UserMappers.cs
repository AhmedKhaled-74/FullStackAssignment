using FullStackAssignment.Application.DTOs.UserDTOs;
using FullStackAssignment.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.Mappers
{
    public static class UserMappers
    {
        public static User ToUserEntity(this RegisterDTO registerDto)
        {
            var user = new User()
            {
                UserName = registerDto.UserName!,
                EmailAddress = registerDto.Email!,
                HashedPassword = PasswordHelper.ToHashedPassword(registerDto.Password!),
                LastLoginDate = DateTime.UtcNow,
            };
            return user;
        }

    }
}
