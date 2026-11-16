using BankSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSim.Infrastructure.Persistence.Configurations;

public sealed class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        // Table-per-hierarchy: one `Accounts` table, a discriminator column selects the concrete type.
        builder.HasDiscriminator<string>("AccountType")
            .HasValue<CheckingAccount>(nameof(AccountType.Checking))
            .HasValue<SavingsAccount>(nameof(AccountType.Savings));

        builder.ComplexProperty(a => a.AccountNumber, nb =>
        {
            nb.Property(n => n.Value).HasColumnName("AccountNumber").IsRequired().HasMaxLength(32);
        });

        // Money is a `record struct` — a complex type (EF Core 8+), not an owned entity type, since
        // it has no identity of its own and OwnsOne requires a reference type.
        builder.ComplexProperty(a => a.Balance, mb =>
        {
            mb.Property(m => m.Amount).HasColumnName("BalanceAmount").HasPrecision(18, 2);
            mb.Property(m => m.Currency).HasColumnName("BalanceCurrency").HasMaxLength(3);
        });

        builder.Property(a => a.OpenedDate).IsRequired();

        // Account.Transactions mirrors the Customer.Accounts pattern: EF writes through the private `_transactions` field.
        builder.HasMany(a => a.Transactions)
            .WithOne()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class CheckingAccountConfiguration : IEntityTypeConfiguration<CheckingAccount>
{
    public void Configure(EntityTypeBuilder<CheckingAccount> builder)
    {
        builder.ComplexProperty(a => a.OverdraftLimit, mb =>
        {
            mb.Property(m => m.Amount).HasColumnName("OverdraftLimitAmount").HasPrecision(18, 2);
            mb.Property(m => m.Currency).HasColumnName("OverdraftLimitCurrency").HasMaxLength(3);
        });
    }
}

public sealed class SavingsAccountConfiguration : IEntityTypeConfiguration<SavingsAccount>
{
    public void Configure(EntityTypeBuilder<SavingsAccount> builder)
    {
        builder.Property(a => a.AnnualInterestRatePercent).HasPrecision(5, 2);
        builder.Property(a => a.LastInterestAccrualDate).IsRequired();
    }
}
