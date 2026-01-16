using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Infrastructure.Services;

public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

    public ReservationCleanupService(IServiceScopeFactory scopeFactory, ILogger<ReservationCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var now = DateTime.UtcNow;
                var expired = await uow.StockReservations.GetExpiredReservationsAsync(now);
                if (expired.Count > 0)
                {
                    await uow.StockReservations.ReleaseRangeAsync(expired);
                    await uow.SaveChangesAsync();
                    _logger.LogInformation("Released {Count} expired stock reservations", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clean expired reservations");
            }

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
    }
}
