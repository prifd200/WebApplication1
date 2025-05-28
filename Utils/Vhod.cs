using System.Security.Claims;
using WebApplication1.Models;
public class AuthService
{
    private AppDbContext _context;

    public AuthService(AppDbContext context)
    {
        _context = context;
    }
    public bool Register(string username, string email, string role, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Логин и пароль не могут быть пустыми.");

        if (_context.Users.Any(u => u.Username == username || u.Email == email))
            return false;

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            Email = email,
            Role= role,
            FirstName = firstName,
            LastName = lastName,
            PasswordHash = passwordHash

        };

        _context.Users.Add(user);
        _context.SaveChanges();

        return true;
    }
    public User Login(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Логин и пароль не могут быть пустыми.");

        var user = _context.Users.FirstOrDefault(u => u.Username == username);

        if (user == null)
            return null;

        if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return user;

        return null;
    }
    public bool ValidateUser(string username, string password)
    {
        return username == "admin" && password == "123456";
    }
    public bool IsEmailUnique(string email)
    {
        return !_context.Users.Any(u => u.Email == email);
    }
    public User GetUserById(int id)
    {
        return _context.Users.Find(id);
    }
    public ClaimsIdentity CreateClaimsIdentity(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };
        return new ClaimsIdentity(claims, "MyCookieAuth");
    }
    
}
