using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebApp.DataAccess.Accessors;
using MusicWebApp.DataAccess.Filters;
using MusicWebApp.Models;
using MusicWebApp.ViewModels;

namespace MusicWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        readonly UserDAO userDAO;
        private const int pageSize = 10;
        
        public UsersController(UserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        public IActionResult Index(int page = 1, UserFilter? filter = null)
        {
            filter ??= new UserFilter();
            int count = (int)Math.Ceiling(userDAO.CountFiltered(filter) / (float)pageSize);
            int showPage = Math.Min(page, count);
            var model = new UsersViewModel()
            {
                Filter = filter,
                Users = userDAO.ReadPage(showPage, pageSize, filter),
                CurrentPage = page,
                MaxPage = count
            };
            return View(model);
        }

        public IActionResult Show(int id)
        {
            User? user = userDAO.Read(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            User? user = userDAO.Read(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        public IActionResult Update(User user)
        {
            if (string.IsNullOrEmpty(user.Password))
            {
                user.Password = userDAO.Read(user.Id).Password;
            }
            else
            {
                user.Password = Models.User.HashPassword(user.Password);
            }
            userDAO.Update(user);
            return RedirectToAction("Show", new {id = user.Id});
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            userDAO.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
