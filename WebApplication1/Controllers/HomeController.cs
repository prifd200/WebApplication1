using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly OnlineUsersService _onlineUsersService;
        public ServiceRequestsController(AppDbContext context, OnlineUsersService onlineUsersService)
        {
            _onlineUsersService = onlineUsersService;
            _context = context;
        }
        private AppDbContext _context;

        // GET: ServiceRequests
        [Authorize]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            if (User.Identity.IsAuthenticated)
            {
                var username =  $"{User.FindFirst("FirstName")?.Value} {User.FindFirst("LastName")?.Value}" ;
                var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0";
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                _onlineUsersService.UpdateUser(username, email, id);
            }
            int pageSize = 10;
            var tickets = _context.Tickets.Include(t => t.Comments)
            .ThenInclude(c => c.Author).AsQueryable();
            var paginatedTickets = await PaginatedList<Ticket>.CreateAsync(tickets, pageNumber ?? 1, pageSize);

            var users = _onlineUsersService.GetOnlineUsers();
            ViewBag.OnlineUsers = users;
            return View(paginatedTickets);
        }

        // GET: ServiceRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ServiceRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket serviceRequest)
        {
            if (ModelState.IsValid)
            {
                serviceRequest.CreatedDate = DateTime.Now;
                serviceRequest.Status = serviceRequest.Status;
                _context.Add(serviceRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var serviceRequest = await _context.Tickets.FindAsync(id);
            if (serviceRequest == null)
            {
                return NotFound();
            }
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket serviceRequest)
        {
            if (id != serviceRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(serviceRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceRequestExists(serviceRequest.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(serviceRequest);
        }

        private bool ServiceRequestExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }
    }
    
    }