using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.DataAccess.Filters
{
    public class TrackFilter
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Track name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mininal year")]
        public int MinYear { get; set; }

        [Display(Name = "Maximal year")]
        public int MaxYear { get; set; } = int.MaxValue;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Author name")]
        public string Author { get; set; } = string.Empty;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Album name")]
        public string Album { get; set; } = string.Empty;

        [Display(Name = "Genre name")]
        public int Genre { get; set; }
    }
}
