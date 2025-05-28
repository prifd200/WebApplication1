using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using MailKit.Security;

namespace WebApplication1.Controllers
{
    public class ReportController : Controller
    {
        private AppDbContext _context;
        public ReportController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> AllReports(int id, int? pageNumber)
        {

            var reports = _context.Reports
                .OrderByDescending(r => r.ReportDate);
                //.ToList();
            int pageSize = 10;
            //var reports = _context.Tickets.AsQueryable();
            var paginatedTickets = await PaginatedList<Report>.CreateAsync(reports, pageNumber ?? 1, pageSize);
            return View(paginatedTickets);
        }

        [HttpGet]
        [Authorize]
        public IActionResult GenerateReport(int id)
        {
            var ticket = _context.Tickets.Find(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var model = new ReportViewModel
            {
                TicketId = ticket.Id,
                TicketDescription = ticket.Description,
                FullName = $"{User.FindFirst("FirstName")?.Value} {User.FindFirst("LastName")?.Value}"
            };

            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateReport(ReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var report = new Report
            {
                TicketId =model.TicketId,
                TicketDescription = model.Description,
                FullName = model.FullName,
                EmailTo = model.EmailTo,
                NewStatus = model.NewStatus,
                Description = model.Description,
                TimeSpentHours = model.TimeSpentHours, 
                
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            bool success = await SendReport(model);
            if (success)
            {
                var ticket = _context.Tickets.Find(model.TicketId);
                if (ticket == null)
                {
                    return NotFound();
                }
                ticket.Status = model.NewStatus;
                _context.Tickets.Update(ticket);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Отчёт успешно отправлен.";
                return RedirectToAction("Index", "");
            }
            ModelState.AddModelError("", "Не удалось отправить отчёт.");
            return View(model);
        }

        private async Task<bool> SendReport(ReportViewModel model)
        {
            try
            {
                using var emailmessage = new MimeMessage();
                emailmessage.From.Add(new MailboxAddress("Система отчётов", "Prifd200@yandex.ru"));
                emailmessage.To.Add(new MailboxAddress("", model.EmailTo));
                emailmessage.Subject = "Отчет по заявке";
                emailmessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = $@"
                    ФИО: {model.FullName}
                    Email: {model.EmailTo}
                    Описание: {model.Description}
                    Затраченное время: {model.TimeSpentHours} часов"
                };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("smtp.yandex.ru", 587, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync("Prifd200@yandex.ru", "vbfxwjbutsniunyw");
                    await client.SendAsync(emailmessage);

                    await client.DisconnectAsync(true);
                }
       
                return true;
            }
            catch (Exception ex)
            {
                // Логирование ошибок
                Console.WriteLine("Ошибка при отправке: " + ex.Message);
                return false;
            }
        }
    
    }
}
