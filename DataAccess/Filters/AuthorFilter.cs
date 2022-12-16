using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.DataAccess.Filters
{
    public class AuthorFilter
    {
        [Display(Name="Author name")]
        public string Name { get; set; } = string.Empty;
    }
}
