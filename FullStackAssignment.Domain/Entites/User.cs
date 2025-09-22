using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Domain.Entites
{
    public class User
    {
        [Key]
        public string UserName { get; set; } = null!;
        public string EmailAddress { get; set; } = null!;
        public string HashedPassword { get; set; } = null!;
        public DateTime LastLoginDate { get; set; }

        // Authentication
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}
