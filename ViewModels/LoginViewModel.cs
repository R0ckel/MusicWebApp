using MusicWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login is required!")]
        public string? Login { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; set; }
    }
}
