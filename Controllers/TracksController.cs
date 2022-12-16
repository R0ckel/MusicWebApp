using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using MusicWebApp.ViewModels;

namespace MusicWebApp.Controllers
{
    public class TracksController : Controller
    {
        readonly TrackDAO trackDAO;
        readonly AuthorDAO authorDAO;
        readonly GenreDAO genreDAO;
        readonly AlbumDAO albumDAO;
        private const int pageSize = 10;

        public TracksController(TrackDAO trackDAO, AuthorDAO authorDAO, GenreDAO genreDAO, AlbumDAO albumDAO)
        {
            this.trackDAO = trackDAO;
            this.authorDAO = authorDAO;
            this.genreDAO = genreDAO;
            this.albumDAO = albumDAO;
        }

        public IActionResult Index(int page = 1, TrackFilter? filter = null)
        {
            filter ??= new TrackFilter();
            int count = (int)Math.Ceiling(trackDAO.CountFiltered(filter) / (float)pageSize);
            int showPage = Math.Min(page, count);
            var model = new TracksViewModel()
            {
                Filter = filter,
                Tracks = trackDAO.ReadPage(showPage, pageSize, filter),
                CurrentPage = showPage,
                MaxPage = count,
                AllGenres = genreDAO.ReadAll()
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult Show(int id)
        {
            Track? track = trackDAO.Read(id);
            if (track == null) return NotFound();
            return View(track);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(int id, TrackFormViewModel trackFVM)
        {
            Track? track = trackDAO.Read(id);
            if (track == null) return NotFound();

            trackFVM.AllAuthors = authorDAO.ReadAll();
            trackFVM.TrackModel ??= track;
            trackFVM.AllGenres = genreDAO.ReadAll();
            trackFVM.AllAlbums = albumDAO.ReadAll();
            if (trackFVM.AuthorCount == 0) trackFVM.AuthorCount = track.Authors.Count();
            return View(trackFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Update(TrackFormViewModel trackFVM)
        {
            if (ModelState.IsValid)
            {
                trackDAO.Update(trackFVM.TrackModel);
                return RedirectToAction("Show", new { id = trackFVM.TrackModel.Id });
            }
            return View(trackFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            trackDAO.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(TrackFormViewModel trackFVM)
        {
            trackFVM.AllAuthors = authorDAO.ReadAll();
            trackFVM.TrackModel ??= new Track();
            trackFVM.AllGenres = genreDAO.ReadAll();
            trackFVM.AllAlbums = albumDAO.ReadAll();
            if (trackFVM.AuthorCount == 0) trackFVM.AuthorCount = 1;
            return View(trackFVM);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(TrackFormViewModel model, int diff = 0)
        {
            if (ModelState.IsValid)
            {
                trackDAO.Create(model.TrackModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckYear(int year)
        {
            if (year > DateTime.Now.Year + 1 || year < -10000)
                return Json(false);
            return Json(true);
        }
    }
}
