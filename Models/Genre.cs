using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [MinLength(2, ErrorMessage = "Genre name is too short. Must be between 2 and 50 symbols")]
        [MaxLength(50, ErrorMessage = "Genre name is too long. Must be between 2 and 50 symbols")]
        public string? Name { get; set; }
        public int TrackCount { get; set; } = 0;
    }
}
