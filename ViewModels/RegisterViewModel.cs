using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Login is required!")]
        [MinLength(4, ErrorMessage = "Login is too short. Must be between 4 and 16 symbols")]
        [MaxLength(16, ErrorMessage = "Login is too long. Must be between 4 and 16 symbols")]
        public string? Login { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password is required!")]
        [MinLength(3, ErrorMessage = "Password is too short. Must be between 8 and 32 symbols")]
        [MaxLength(32, ErrorMessage = "Password is too long. Must be between 8 and 32 symbols")]
        //TO DO: Edit MinLength after tests!
        public string? Password { get; set; }

        [Display(Name = "Repeat password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don`t match")]
        public string? ConfirmPassword { get; set; }
    }
}
