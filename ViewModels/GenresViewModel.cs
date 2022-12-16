using MusicWebApp.DataAccess;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;

namespace MusicWebApp.ViewModels
{
    public class GenresViewModel
    {
        public IEnumerable<Genre> Genres { get; set; }
        public GenreFilter Filter { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
    }
}
