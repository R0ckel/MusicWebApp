using MusicWebApp.Models;

namespace MusicWebApp.DataAccess.Filters
{
    public class UserFilter
    {
        public string Login { get; set; } = string.Empty;
        public int Role { get; set; }
    }
}
