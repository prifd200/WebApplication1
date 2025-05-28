using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    private readonly OnlineUsersService _onlineUsersService;

    public UserController(OnlineUsersService onlineUsersService)
    {
        _onlineUsersService = onlineUsersService;
    }

    [Authorize] 
    public IActionResult OnlineUsers()
    {
        var users = _onlineUsersService.GetOnlineUsers();
        Console.WriteLine(User);
        return View(users);
    }
}