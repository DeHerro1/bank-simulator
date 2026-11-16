using BankSim.Domain.Events;
using BankSim.Domain.Exceptions;
using BankSim.Domain.Interfaces;
using BankSim.Domain.ValueObjects;

namespace BankSim.Domain.Entities;

/// <summary>
/// Base type for every account. Holds the shared ledger/balance mechanics and raises domain
/// events; concrete subclasses (CheckingAccount, SavingsAccount) supply the withdrawal rule
/// that differs between them (overdraft vs. no-overdraft).
/// </summary>
public abstract class Account : IAccount
{
    /// <summary>Withdrawals at or above this amount raise <see cref="LargeWithdrawalOccurred"/>.</summary>
    public static readonly Money LargeWithdrawalThreshold = new(1_000m);

    /// <summary>Balances at or below this amount raise <see cref="LowBalanceDetected"/>.</summary>
    public static readonly Money LowBalanceThreshold = new(50m);

    private readonly List<TransactionRecord> _transactions = [];

    public Guid Id { get; init; } = Guid.NewGuid();
    public AccountNumber AccountNumber { get; init; } = AccountNumber.NewRandom();
    public Guid CustomerId { get; init; }
    public Money Balance { get; protected set; }
    public DateTimeOffset OpenedDate { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyList<TransactionRecord> Transactions => _transactions;

    public event EventHandler<LargeWithdrawalEventArgs>? LargeWithdrawalOccurred;
    public event EventHandler<LowBalanceEventArgs>? LowBalanceDetected;

    /// <summary>EF Core materialization constructor.</summary>
    protected Account() { }

    protected Account(Guid customerId, Money openingBalance)
    {
        CustomerId = customerId;
        Balance = openingBalance;
    }

    public bool IsOverdrawn => Balance.IsNegative;

    public void Deposit(Money amount) => Deposit(amount, TransactionType.Deposit, relatedAccountId: null);

    /// <summary>Deposits with an explicit transaction type/related account, used by transfers (TransferIn).</summary>
    public void Deposit(Money amount, TransactionType type, Guid? relatedAccountId)
    {
        EnsurePositiveAmount(amount);

        Balance += amount;
        RecordTransaction(type, amount, relatedAccountId);
    }

    public void Withdraw(Money amount) => Withdraw(amount, TransactionType.Withdrawal, relatedAccountId: null);

    /// <summary>Withdraws with an explicit transaction type/related account, used by transfers (TransferOut).</summary>
    public void Withdraw(Money amount, TransactionType type, Guid? relatedAccountId)
    {
        EnsurePositiveAmount(amount);
        ValidateWithdrawal(amount);
        ApplyWithdrawal(amount, type, relatedAccountId);
    }

    /// <summary>Withdrawal rules (overdraft allowance, withdrawal caps, etc.) differ per account type.</summary>
    protected abstract void ValidateWithdrawal(Money amount);

    /// <summary>Applies an already-validated withdrawal: updates the balance, records the ledger entry, and raises events.</summary>
    private void ApplyWithdrawal(Money amount, TransactionType type, Guid? relatedAccountId)
    {
        Balance -= amount;
        RecordTransaction(type, amount, relatedAccountId);

        if (amount >= LargeWithdrawalThreshold)
        {
            LargeWithdrawalOccurred?.Invoke(this, new LargeWithdrawalEventArgs
            {
                AccountId = Id,
                Amount = amount,
                BalanceAfter = Balance
            });
        }

        if (Balance <= LowBalanceThreshold)
        {
            LowBalanceDetected?.Invoke(this, new LowBalanceEventArgs
            {
                AccountId = Id,
                Balance = Balance,
                Threshold = LowBalanceThreshold
            });
        }
    }

    internal void RecordTransaction(TransactionType type, Money amount, Guid? relatedAccountId = null)
    {
        _transactions.Add(new TransactionRecord
        {
            AccountId = Id,
            Type = type,
            Amount = amount,
            BalanceAfter = Balance,
            RelatedAccountId = relatedAccountId
        });
    }

    protected static void EnsurePositiveAmount(Money amount)
    {
        if (amount.Amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
    }
}
