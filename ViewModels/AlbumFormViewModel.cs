using MusicWebApp.Models;
using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.ViewModels
{
    public class AlbumFormViewModel
    {
        public Album AlbumModel { get; set; }

        public IEnumerable<Author> AllAuthors { get; set; } = new List<Author>();

        [Display(Name ="Number of authors")]
        [Range(0,10, ErrorMessage ="Number of authors must be between 0 and 10")]
        public int AuthorCount { get; set; } = 0;
    }
}
