using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Внешние ключи
        public int TicketId { get; set; }
        public int AuthorId { get; set; }

        // Навигационные свойства (опционально)
        public virtual Ticket Ticket { get; set; }
        public virtual User Author { get; set; }
    }
}
