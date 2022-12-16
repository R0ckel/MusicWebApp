using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;

namespace MusicWebApp.ViewModels
{
    public class AlbumsViewModel
    {
        public IEnumerable<Album> Albums { get; set; }
        public AlbumFilter Filter { get; set; } = new AlbumFilter();
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
    }
}
