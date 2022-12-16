using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebApp.DataAccess;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using MusicWebApp.ViewModels;
using System.Data;
using System.Reflection;

namespace MusicWebApp.Controllers
{
    public class AuthorsController : Controller
    {
        readonly AuthorDAO authorDAO;
        private const int pageSize = 10;

        public AuthorsController(AuthorDAO authorDAO)
        {
            this.authorDAO = authorDAO;
        }

        public IActionResult Index(int page = 1, AuthorFilter? filter = null)
        {
            filter ??= new AuthorFilter();
            int count = (int)Math.Ceiling(authorDAO.CountFiltered(filter) / (float)pageSize);
            int showPage = Math.Min(page, count);
            var model = new AuthorsViewModel()
            {
                Filter = filter,
                Authors = authorDAO.ReadPage(showPage, pageSize, filter),
                CurrentPage = showPage,
                MaxPage = count
            };
            return View(model);
        }

        public IActionResult Show(int id)
        {
            Author? author = authorDAO.Read(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id)
        {
            Author? author = authorDAO.Read(id);
            if (author == null) return NotFound();
            return View(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(Author author)
        {
            if (ModelState.IsValid)
            {
                Author? dbauthor = authorDAO.ReadByName(author.Name);
                if (dbauthor == null || dbauthor.Id == author.Id)
                {
                    authorDAO.Update(author);
                    return RedirectToAction("Show", new { id = author.Id });
                }
                else
                {
                    ModelState.AddModelError("Name", "Author with this name already exists!");
                }
            }
            return View(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            authorDAO.Delete(id);
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
        public IActionResult Create(Author model)
        {
            if (ModelState.IsValid)
            {
                Author? dbauthor = authorDAO.ReadByName(model.Name);
                if (dbauthor == null)
                {
                    authorDAO.Create(model);
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Name", "Author with this name already exists!");
                }
            }
            return View(model);
        }
    }
}
