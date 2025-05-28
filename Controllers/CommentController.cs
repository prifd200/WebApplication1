using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly AppDbContext _context;

        public CommentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int ticketId, string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                TempData["ErrorMessage"] = "Комментарий не может быть пустым";
                return RedirectToAction("Edit", "Tickets", new { id = ticketId });
            }

            var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"); 
            var comment = new Comment
            {
                Text = text,
                TicketId = ticketId,
                AuthorId = id,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Tickets");
        }
    }
}
