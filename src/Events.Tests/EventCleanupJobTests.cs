using EventService;
using EventService.Repos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Events.Tests;

public class EventCleanupJobTests
{
    [Fact]
    public async Task ExecuteAsync_OnFirstRun_DeletesEventsOlderThanOneYear()
    {
        var repo = Substitute.For<IEventRetentionRepository>();
        var scopeFactory = BuildScopeFactory(repo);
        var logger = Substitute.For<ILogger<EventCleanupJob>>();

        using var cts = new CancellationTokenSource();

        repo.DeleteOlderByCutoffDate(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                cts.Cancel();
                return Task.CompletedTask;
            });

        var options = Options.Create(new EventCleanupOptions { Interval = TimeSpan.FromMilliseconds(1) });
        var job = new EventCleanupJob(scopeFactory, logger, options);

        try { await job.StartAsync(cts.Token); }
        catch (OperationCanceledException) { }

        await repo.Received(1).DeleteOlderByCutoffDate(
            Arg.Is<DateTime>(d => d.Date == DateTime.UtcNow.AddYears(-1).Date),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenRepoThrows_ContinuesWithoutCrashing()
    {
        var repo = Substitute.For<IEventRetentionRepository>();
        var scopeFactory = BuildScopeFactory(repo);
        var logger = Substitute.For<ILogger<EventCleanupJob>>();

        var callCount = 0;
        repo.DeleteOlderByCutoffDate(Arg.Any<DateTime>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                callCount++;
                if (callCount == 1) throw new Exception("Transient DB error");
                return Task.CompletedTask;
            });

        using var cts = new CancellationTokenSource();

        var options = Options.Create(new EventCleanupOptions { Interval = TimeSpan.FromMilliseconds(1) });
        var job = new EventCleanupJob(scopeFactory, logger, options);
        var jobTask = job.StartAsync(cts.Token);

        await Task.Delay(100);
        await cts.CancelAsync();

        var act = async () => await jobTask;
        await act.Should().NotThrowAsync();
    }

    private static IServiceScopeFactory BuildScopeFactory(IEventRetentionRepository repo)
    {
        var provider = Substitute.For<IServiceProvider>();
        provider.GetService(typeof(IEventRetentionRepository)).Returns(repo);

        var scope = Substitute.For<IServiceScope>();
        scope.ServiceProvider.Returns(provider);

        var factory = Substitute.For<IServiceScopeFactory>();
        factory.CreateScope().Returns(scope);
        return factory;
    }
}
