using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.Models
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Track name is too short. Must be between 1 and 50 symbols")]
        [MaxLength(50, ErrorMessage = "Track name is too long. Must be between 1 and 50 symbols")]
        public string Name { get; set; } = string.Empty;

        [Remote(action: "CheckYear", controller: "Tracks", ErrorMessage="Incorrect year!")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Display(Name = "Album")]
        public Album TrackAlbum { get; set; } = new Album();

        public IEnumerable<string> Authors { get; set; } = new List<string>();

        [Required(ErrorMessage = "Select author or 'None' to dismiss any")]
        public IEnumerable<int> AuthorsId { get; set; }//for editing Authors list in DB and only

        [Required(ErrorMessage = "Genre is required!")]
        [Display(Name = "Genre")]
        public Genre TrackGenre { get; set; } = new Genre();

        public void SetAuthorsFromString(string str)
        {
            Authors = str.Split(";").Select(a => a.Trim().Trim('\n'));
        }
        public string GetAuthorsIdAsString()
        {
            if (AuthorsId == null) return string.Empty;
            return string.Join(";", AuthorsId.Distinct().Select(x => x.ToString()).ToArray());
        }
    }
}
