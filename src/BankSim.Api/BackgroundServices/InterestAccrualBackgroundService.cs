using BankSim.Application.Services;

namespace BankSim.Api.BackgroundServices;

/// <summary>
/// Periodically sweeps every savings account and accrues interest. Runs on a `PeriodicTimer` and
/// resolves its dependencies from a fresh DI scope each tick, since `IAccountRepository`/`DbContext`
/// are scoped services and this hosted service itself is a long-lived singleton.
/// </summary>
public sealed class InterestAccrualBackgroundService(
    IServiceScopeFactory scopeFactory, ILogger<InterestAccrualBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan AccrualInterval = TimeSpan.FromHours(24);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(AccrualInterval);

        do
        {
            try
            {
                await AccrueInterestOnceAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Interest accrual sweep failed.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task AccrueInterestOnceAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = scopeFactory.CreateScope();
        var interestService = scope.ServiceProvider.GetRequiredService<IInterestAccrualService>();

        int accrued = await interestService.AccrueInterestForAllAccountsAsync(cancellationToken);
        logger.LogInformation("Interest accrual sweep applied interest to {Count} savings account(s).", accrued);
    }
}
