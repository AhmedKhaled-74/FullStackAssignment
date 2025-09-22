using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStackAssignment.Application.DTOs.UserDTOs
{
    public class LoginDTO
    {
        [Required]
        public string? UserNameOrEmail { get; set; }


        [Required]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Invalid Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
}
