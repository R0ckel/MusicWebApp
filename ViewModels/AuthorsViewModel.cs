using MusicWebApp.DataAccess;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;

namespace MusicWebApp.ViewModels
{
    public class AuthorsViewModel
    {
        public IEnumerable<Author> Authors { get; set; }
        public AuthorFilter Filter { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
    }
}
