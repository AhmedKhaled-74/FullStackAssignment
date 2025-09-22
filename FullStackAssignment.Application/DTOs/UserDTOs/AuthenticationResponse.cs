using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.DTOs.UserDTOs
{
    public class AuthenticationResponse
    {
        public string? UserName { get; set; } 
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpirationDate { get; set; }
        public DateTime Expiration { get; set; }

    }
}
