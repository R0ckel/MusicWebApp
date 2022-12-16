using MusicWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.ViewModels
{
    public class TrackFormViewModel
    {
        public Track TrackModel { get; set; }

        public IEnumerable<Author> AllAuthors { get; set; } = new List<Author>();

        [Display(Name = "Number of authors")]
        [Range(0, 10, ErrorMessage = "Number of authors must be between 0 and 10")]
        public int AuthorCount { get; set; } = 0;
        public IEnumerable<Genre> AllGenres { get; set; } = new List<Genre>();
        public IEnumerable<Album> AllAlbums { get; set; } = new List<Album>();
    }
}
