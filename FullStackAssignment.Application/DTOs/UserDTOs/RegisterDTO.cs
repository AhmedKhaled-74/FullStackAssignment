using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace FullStackAssignment.Application.DTOs.UserDTOs
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 30 characters.")]
        [Remote("CheckUserNameExists", "Account", ErrorMessage = "UserName is in used try another one")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens.")]
        public string? UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Remote("CheckEmailExists", "Account", ErrorMessage = "Email already exists.")]
        public string? Email { get; set; }


        [Required]
        [StringLength(20, MinimumLength = 10, ErrorMessage = "Password must be between 10 and 20 characters.")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[_@*-])[A-Za-z\d_@*-]{10,21}$",
            ErrorMessage = "Password must be 10–21 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character (_, @, -, *).")]
        public string Password { get; set; } = null!;


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

    }
}
