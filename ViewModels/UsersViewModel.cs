using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;

namespace MusicWebApp.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public UserFilter Filter { get; set; }
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }
    }
}
