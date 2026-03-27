using EventService.Repos;
using Microsoft.Extensions.Options;

namespace EventService
{
    public class EventCleanupJob(
        IServiceScopeFactory scopeFactory,
        ILogger<EventCleanupJob> logger,
        IOptions<EventCleanupOptions> options) : BackgroundService
    {
        private TimeSpan Interval => options.Value.Interval;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var repo = scope.ServiceProvider.GetRequiredService<IEventRetentionRepository>();
                    var cutoff = DateTime.UtcNow.Subtract(options.Value.RetentionPeriod);
                    await repo.DeleteOlderByCutoffDate(cutoff, stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Event cleanup failed.");
                }
                await Task.Delay(Interval, stoppingToken);
            }
        }
    }
}
