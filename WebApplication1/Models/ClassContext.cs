using Microsoft.EntityFrameworkCore;
namespace WebApplication1.Models;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {

        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=zaivka;Trusted_Connection=True;encrypt=false;");
    }
    
}
