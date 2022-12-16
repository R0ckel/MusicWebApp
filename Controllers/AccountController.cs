using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using MusicWebApp.Models;
using System.Security.Claims;
using MusicWebApp.ViewModels;
using MusicWebApp.DataAccess.Accessors;

namespace MusicWebApp.Controllers
{
    public class AccountController : Controller
    {
        readonly UserDAO userDAO;

        public AccountController(UserDAO userDAO)
        {
            this.userDAO = userDAO;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = userDAO.ReadByLogin(model.Login);
                if (user != null && user.PasswordMatches(model.Password))
                {
                    await Authenticate(user);
                    if (!string.IsNullOrEmpty(Request.Query["ReturnUrl"]))
                    {
                        return Redirect(Request.Query["ReturnUrl"]);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect login and(or) password!");
            }
            return View(model);
        }
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole.ToString())
            };
            ClaimsIdentity id = new (claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? dbuser = userDAO.ReadByLogin(model.Login);
                if (dbuser == null)
                {
                    User user = new ()
                    {
                        Login = model.Login,
                        Password = Models.User.HashPassword(model.Password),
                        UserRole = Role.User
                    };
                    userDAO.Create(user);
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "User with this login already exists!");
                }
            }
            return View(model);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
