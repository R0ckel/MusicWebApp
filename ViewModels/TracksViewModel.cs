using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;

namespace MusicWebApp.ViewModels
{
    public class TracksViewModel
    {
        public IEnumerable<Track> Tracks { get; set; }
        public TrackFilter Filter { get; set; } = new TrackFilter();
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
        public IEnumerable<Genre> AllGenres { get; set; } = new List<Genre>();
    }
}
