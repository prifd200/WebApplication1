using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Описание обязательно")]
        [Display(Name = "Описание заявки")]
        public string Description { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Статус обязателен")]
        [Display(Name = "Статус заявки")]
        public TicketStatus? Status { get; set; }
        public bool IsPriority { get; set; }
        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Категория обязательна")]
        public TicketCategory? Category { get; set; }
        public int? AssignedEmployeeId { get; set; } 
        [Display (Name = "Сотрудник")]
        public string? AssignedEmployeeName { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
    public enum TicketCategory
    {
        ТехническаяПоддержка,
        Поддержка1С,
        ЗакупкаТехники
    }
    public enum TicketStatus
    {
        Открыта,
        [Display(Name = "В работе")]
        В_работе ,
        Завершена
    }
}