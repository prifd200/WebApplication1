using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Report 
    {
        public int Id { get; set; }
        [Required]
        public int TicketId { get; set; }

        public string TicketDescription { get; set; }

        [Display(Name = "ФИО пользователя")]
        [Required(ErrorMessage = "ФИО обязательно")]
        public string FullName { get; set; }

        [Display(Name = "Email получателя")]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string EmailTo { get; set; }
        [Display(Name = "Новый статус заявки")]
        [Required(ErrorMessage = "Выберите статус")]
        public TicketStatus NewStatus { get; set; }
        [Display(Name = "Описание выполненной работы")]
        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }

        [Display(Name = "Время, затраченное на заявку (в часах)")]
        [Range(0.1, 1000, ErrorMessage = "Время должно быть больше 0")]
        [Required(ErrorMessage = "Укажите время")]
        public double TimeSpentHours { get; set; }
        public DateTime ReportDate { get; set; } = DateTime.Now;

    }
    public class ReportViewModel
    {
        [Required]
        public int TicketId { get; set; }

        public string TicketDescription { get; set; }

        [Display(Name = "ФИО пользователя")]
        [Required(ErrorMessage = "ФИО обязательно")]
        public string FullName { get; set; }

        [Display(Name = "Email получателя")]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат Email")]
        public string EmailTo { get; set; }
        [Display(Name = "Новый статус заявки")]
        [Required(ErrorMessage = "Выберите статус")]
        public TicketStatus NewStatus { get; set; }

        [Display(Name = "Описание выполненной работы")]
        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; }

        [Display(Name = "Время, затраченное на заявку (в часах)")]
        [Range(0.1, 1000, ErrorMessage = "Время должно быть больше 0")]
        [Required(ErrorMessage = "Укажите время")]
        public double TimeSpentHours { get; set; }
    }
}
