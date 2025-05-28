using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _context;
        private AuthService _authService;

        public AccountController(AppDbContext context)
        {
            _context = context;
            _authService = new AuthService(_context);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); 
            }

            bool registrationSuccess = _authService.Register(model.Username, model.Email, model.Role, model.Password, model.FirstName, model.LastName);

            if (registrationSuccess)
            {
                return RedirectToAction("Index", ""); // Перенаправляем на страницу входа
            }

            ModelState.AddModelError(string.Empty, "Пользователь с таким логином или Email уже существует.");
            return View(model);
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var claimsIdentity = _authService.CreateClaimsIdentity(user);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10)
                };
                await HttpContext.SignInAsync("MyCookieAuth", new UserPrincipal(claimsIdentity), authProperties);
                
                return RedirectToAction("Index", "");
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль.");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(string exit)
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "");
        }
        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }

    }
}