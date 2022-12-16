using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MusicWebApp.Models
{
    public class Album
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Album name is required")]
        [MinLength(1, ErrorMessage = "Album name is too short. Must be between 1 and 50 symbols")]
        [MaxLength(50, ErrorMessage = "Album name is too long. Must be between 1 and 50 symbols")]
        public string? Name { get; set; }

        [Remote(action: "CheckYear", controller: "Tracks", ErrorMessage = "Incorrect year!")]
        public int Year { get; set; } = DateTime.Now.Year;

        public IEnumerable<string> Authors { get; set; } = new List<string>();

        //[Required(ErrorMessage = "Select author or 'None' to dismiss any")]
        public IEnumerable<int> AuthorsId { get; set; } = new List<int>(); //for editing Authors list in DB and only

        public void SetAuthorsFromString(string str)
        {
            Authors = str.Split(";").Select(a => a.Trim().Trim('\n'));
        }
        public string GetAuthorsIdAsString()
        {
            return string.Join(";", AuthorsId.Distinct().Select(x => x.ToString()).ToArray());
        }
        public string GetAuthorsAsString()
        {
            return string.Join("; ", Authors.Select(x => x.ToString()).ToArray());
        }
    }
}
