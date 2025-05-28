using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
namespace WebApplication1.Controllers
{
    public class TicketsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly OnlineUsersService _onlineUsersService;
        public TicketsController(AppDbContext context, OnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;

            _context = context;

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(Ticket model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ticket = new Ticket
            {
                Description = model.Description,
                CreatedDate = DateTime.Now,
                Status = model.Status,
                IsPriority = model.IsPriority,
                Category = model.Category
            };
            if (model.Status == TicketStatus.В_работе || model.Status == TicketStatus.Завершена)
            {
                var employeeName = User.Identity.IsAuthenticated ? $"{User.FindFirst("FirstName")?.Value} {User.FindFirst("LastName")?.Value}" : "Неизвестный";
                ticket.AssignedEmployeeName = employeeName;
                ticket.AssignedEmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            }
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявка успешно создана!";
            return RedirectToAction("Index", "");
        }
        public enum SortOrder
        {
            Asc,
            Desc
        }
        public async Task<IActionResult> Index(string searchString, TicketStatus? statusFilter, TicketCategory? categoryFilter, int? pageNumber, bool isPriorityFilter = false, SortOrder dateSortOrder = SortOrder.Desc)
        {
            var tickets = _context.Tickets.Include(t => t.Comments)
            .ThenInclude(c => c.Author).AsQueryable();
            if (User.Identity.IsAuthenticated)
            {
                var username = $"{User.FindFirst("FirstName")?.Value} {User.FindFirst("LastName")?.Value}";
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                _onlineUsersService.UpdateUser(username, email, id);
            }

            var users = _onlineUsersService.GetOnlineUsers();
            ViewBag.OnlineUsers = users;
        
            if (!string.IsNullOrEmpty(searchString))
            {
                tickets = tickets.Where(t => t.Description.Contains(searchString));
            }
            if (statusFilter.HasValue)
            {
                tickets = tickets.Where(t => t.Status == statusFilter.Value);
            }
            if (isPriorityFilter)
            {
                tickets = tickets.Where(t => t.IsPriority);
            }
            if (categoryFilter.HasValue)
            {
                tickets = tickets.Where(t => t.Category == categoryFilter.Value);
            }
            if (dateSortOrder == SortOrder.Asc)
            {
                tickets = tickets.OrderBy(t => t.CreatedDate);
            }
            else
            {
                tickets = tickets.OrderByDescending(t => t.CreatedDate);
            }
            int pageSize = 10;
            var paginatedTickets = await PaginatedList<Ticket>.CreateAsync(tickets, pageNumber ?? 1, pageSize);
            ViewBag.SelectedStatus = statusFilter;
            ViewBag.DateSortOrder = (int)dateSortOrder;
            ViewBag.SearchString = searchString;
            ViewBag.SelectedCategory = categoryFilter;
            ViewBag.IsPriorityFilter = isPriorityFilter ;
            return View(paginatedTickets); 
        }
        public IActionResult Delete(int id)
        {
            var ticket = _context.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            var ticket = _context.Tickets.Include(t => t.Comments)
            .ThenInclude(c => c.Author)
        .FirstOrDefault(t => t.Id == id); ;
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Заявка успешно удалена.";
            return RedirectToAction("Index", "");
        }
        public IActionResult Edit(int id)
        {
            var ticket = _context.Tickets
        .Include(t => t.Comments)
            .ThenInclude(c => c.Author)
        .FirstOrDefault(t => t.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            var model = new Ticket
            {
                Description = ticket.Description,
                Status = ticket.Status,
                Comments = ticket.Comments,
                IsPriority = ticket.IsPriority,
                Category = ticket.Category

            };

            ViewBag.Id = ticket.Id;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Ticket model)
        {
            var ticket = _context.Tickets.Include(t => t.Comments)
            .ThenInclude(c => c.Author)
        .FirstOrDefault(t => t.Id == id); ;
            if (ticket == null) 
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Id = id;
                return View(model);
            }
            ticket.Description = model.Description;
            ticket.Status = model.Status;
            ticket.IsPriority = model.IsPriority;
            ticket.Category = model.Category;
            if (model.Status == TicketStatus.В_работе || model.Status == TicketStatus.Завершена)
            {
                var employeeName = User.Identity.IsAuthenticated ? $"{User.FindFirst("FirstName")?.Value} {User.FindFirst("LastName")?.Value}" : "Неизвестный";
                ticket.AssignedEmployeeName = employeeName;
                ticket.AssignedEmployeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            }
            else 
            {
                ticket.AssignedEmployeeName = "";
            }
            _context.Tickets.Update(ticket);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Заявка успешно обновлена.";
            return RedirectToAction("Index", "");
        }
        [HttpGet]
        [Authorize]
        public IActionResult ReportDetails(int id)
        {
            var report = _context.Reports.Find(id);

            if (report == null)
                return NotFound();

            return View(report);
        }
        public IActionResult Details(int id)
        {
            var ticket = _context.Tickets
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefault(t => t.Id == id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }
    }

}