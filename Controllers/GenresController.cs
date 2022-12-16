using Microsoft.AspNetCore.Mvc;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.Models;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace MusicWebApp.Controllers
{
    public class GenresController : Controller
    {
        readonly GenreDAO genreDAO;
        private const int pageSize = 10;

        public GenresController(GenreDAO genreDAO)
        {
            this.genreDAO = genreDAO;
        }

        public IActionResult Index(int page = 1, GenreFilter? filter = null)
        {
            filter ??= new GenreFilter();
            int count = (int)Math.Ceiling(genreDAO.CountFiltered(filter) / (float)pageSize);
            int showPage = Math.Min(page, count);
            var model = new GenresViewModel()
            {
                Filter = filter,
                Genres = genreDAO.ReadPage(showPage, pageSize, filter),
                CurrentPage = showPage,
                MaxPage = count
            };
            return View(model);
        }

        public IActionResult Show(int id)
        {
            Genre? genre = genreDAO.Read(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            Genre? genre = genreDAO.Read(id);
            if (genre == null) return NotFound();
            return View(genre);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Genre genre)
        {
            genreDAO.Update(genre);
            return RedirectToAction("Show", new { id = genre.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            genreDAO.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Genre model)
        {
            if (ModelState.IsValid)
            {
                Genre? dbgenre = genreDAO.ReadByName(model.Name);
                if (dbgenre == null)
                {
                    genreDAO.Create(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Genre with this name already exists!");
                }
            }
            return View(model);
        }
    }
}
