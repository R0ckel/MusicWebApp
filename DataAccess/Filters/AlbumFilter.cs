using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.DataAccess.Filters
{
    public class AlbumFilter
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Album name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Mininal year")]
        public int MinYear { get; set; }

        [Display(Name = "Maximal year")]
        public int MaxYear { get; set; } = int.MaxValue;

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Display(Name = "Author name")]
        public string Author { get; set; } = string.Empty;
    }
}
