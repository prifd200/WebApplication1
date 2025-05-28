using System;
using System.Collections.Concurrent;
using System.Linq;
using WebApplication1.Models;

public class OnlineUsersService
{
    // Используем потокобезопасный словарь
    private static readonly ConcurrentDictionary<string, OnlineUser> _users = new();

    public void UpdateUser(string username, string email, string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        _users[id] = new OnlineUser
        {
            Username = username,
            Email = email,
            LastActive = DateTime.Now
        };
    }

    public void RemoveUser(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        _users.TryRemove(id, out _);
    }

    public IEnumerable<OnlineUser> GetOnlineUsers()
    {
        // Удаляем пользователей, которые неактивны больше 5 минут
        var now = DateTime.Now;
        var inactiveUsers = _users.Where(u => (now - u.Value.LastActive).TotalMinutes > 5).ToList();
        foreach (var user in inactiveUsers)
        {
            _users.TryRemove(user.Key, out _);
        }

        return _users.Values.OrderBy(u => u.Username);
    }
}