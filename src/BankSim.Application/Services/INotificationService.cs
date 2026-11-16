using BankSim.Domain.Events;

namespace BankSim.Application.Services;

/// <summary>Reacts to domain events raised by an <see cref="BankSim.Domain.Entities.Account"/> during a unit of work.</summary>
public interface INotificationService
{
    void OnLargeWithdrawal(object? sender, LargeWithdrawalEventArgs e);
    void OnLowBalance(object? sender, LowBalanceEventArgs e);
}
