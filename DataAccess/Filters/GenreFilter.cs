using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.DataAccess.Filters
{
    public class GenreFilter
    {
        [Display(Name = "Genre Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Minimum tracks")]
        public int MinTracks { get; set; }

        [Display(Name = "Maximun tracks")]
        public int MaxTracks { get; set; } = int.MaxValue;
    }
}
