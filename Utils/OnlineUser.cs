using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

public class CleanupHostedService : IHostedService
{
    private readonly OnlineUsersService _onlineUsersService;

    public CleanupHostedService(OnlineUsersService onlineUsersService)
    {
        _onlineUsersService = onlineUsersService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _onlineUsersService.GetOnlineUsers(); // Это очищает старые записи
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }, cancellationToken);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}