using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using MusicWebApp.ViewModels;

namespace MusicWebApp.Controllers
{
    public class AlbumsController : Controller
    {
        readonly AlbumDAO albumDAO;
        readonly AuthorDAO authorDAO;
        private const int pageSize = 10;

        public AlbumsController(AlbumDAO albumDAO, AuthorDAO authorDAO)
        {
            this.albumDAO = albumDAO;
            this.authorDAO = authorDAO;
        }

        public IActionResult Index(int page = 1, AlbumFilter? filter = null)
        {
            filter ??= new AlbumFilter();
            int count = (int)Math.Ceiling(albumDAO.CountFiltered(filter) / (float)pageSize);
            int showPage = Math.Min(page, count);
            var model = new AlbumsViewModel()
            {
                Filter = filter,
                Albums = albumDAO.ReadPage(showPage, pageSize, filter),
                CurrentPage = showPage,
                MaxPage = count
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Album? album = albumDAO.Read(id);
            if (album == null) return NotFound();
            return View(album);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, AlbumFormViewModel albumFVM)
        {
            Album? album = albumDAO.Read(id);
            if (album == null) return NotFound();

            albumFVM.AllAuthors = authorDAO.ReadAll();
            albumFVM.AlbumModel ??= album;
            if (albumFVM.AuthorCount == 0) albumFVM.AuthorCount = album.Authors.Count();
            return View(albumFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(AlbumFormViewModel albumFVM)
        {
            if (ModelState.IsValid)
            {
                albumDAO.Update(albumFVM.AlbumModel);
                return RedirectToAction("Show", new { id = albumFVM.AlbumModel.Id });
            }
            albumFVM.AllAuthors = authorDAO.ReadAll();
            return View(albumFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            albumDAO.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(AlbumFormViewModel albumFVM)
        {
            albumFVM.AllAuthors = authorDAO.ReadAll();
            albumFVM.AlbumModel ??= new Album();
            if (albumFVM.AuthorCount == 0) albumFVM.AuthorCount = 1;
            return View(albumFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(AlbumFormViewModel model, int diff = 0)
        {
            if (ModelState.IsValid)
            {
                albumDAO.Create(model.AlbumModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}
