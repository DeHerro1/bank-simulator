using BankSim.Application.Services;
using BankSim.Domain.Events;

namespace BankSim.Api.Notifications;

/// <summary>
/// Subscribes to the domain events an <see cref="BankSim.Domain.Entities.Account"/> raises and
/// "sends an alert" (here, just logs it) — stands in for a real email/SMS/push notification gateway.
/// </summary>
public sealed class ConsoleNotificationService(ILogger<ConsoleNotificationService> logger) : INotificationService
{
    public void OnLargeWithdrawal(object? sender, LargeWithdrawalEventArgs e) =>
        logger.LogInformation(
            "ALERT: large withdrawal of {Amount} on account {AccountId}, balance now {Balance}",
            e.Amount, e.AccountId, e.BalanceAfter);

    public void OnLowBalance(object? sender, LowBalanceEventArgs e) =>
        logger.LogInformation(
            "ALERT: low balance on account {AccountId}: {Balance} (threshold {Threshold})",
            e.AccountId, e.Balance, e.Threshold);
}
