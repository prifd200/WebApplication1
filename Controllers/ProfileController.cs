using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(int.Parse(userId));

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            return View("Profile",user);
        }
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(int.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash);

            if (!isPasswordValid)
            {
                ModelState.AddModelError("", "Текущий пароль указан неверно.");
                return View(model);
            }

            // Хэшируем и сохраняем новый пароль
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Устанавливаем сообщение об успехе
            TempData["SuccessMessage"] = "Пароль успешно изменён.";

            return RedirectToAction("Index", "");
        }
        [Authorize]
        public IActionResult Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(int.Parse(userId));

            if (user == null) return NotFound();

            var model = new EditProfileViewModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                NewEmail = user.Email
            };

            return View("EditProfile",model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(EditProfileViewModel model)
        {
            if (!ModelState.IsValid) return View("EditProfile", model);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = _context.Users.Find(int.Parse(userId));

            if (user == null) return NotFound();
            if (_context.Users.Any(u => u.Email == model.NewEmail && u.Id != user.Id))
            {
                ModelState.AddModelError("NewEmail", "Этот Email уже используется.");
                return View("EditProfile",model);
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.NewEmail != null ? model.NewEmail : user.Email;
            _context.Users.Update(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Данные успешно обновлены.";

            return RedirectToAction("Index");
        }
    }

}