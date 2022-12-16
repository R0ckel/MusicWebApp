using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [MinLength(1, ErrorMessage = "Author name is too short. Must be between 1 and 50 symbols")]
        [MaxLength(50, ErrorMessage = "Author name is too long. Must be between 1 and 50 symbols")]
        public string? Name { get; set; }
    }
}
