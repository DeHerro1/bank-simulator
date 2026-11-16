using BankSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSim.Infrastructure.Persistence.Configurations;

public sealed class TransactionRecordConfiguration : IEntityTypeConfiguration<TransactionRecord>
{
    public void Configure(EntityTypeBuilder<TransactionRecord> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(32);
        builder.Property(t => t.Timestamp).IsRequired();

        builder.ComplexProperty(t => t.Amount, mb =>
        {
            mb.Property(m => m.Amount).HasColumnName("Amount").HasPrecision(18, 2);
            mb.Property(m => m.Currency).HasColumnName("Currency").HasMaxLength(3);
        });

        builder.ComplexProperty(t => t.BalanceAfter, mb =>
        {
            mb.Property(m => m.Amount).HasColumnName("BalanceAfterAmount").HasPrecision(18, 2);
            mb.Property(m => m.Currency).HasColumnName("BalanceAfterCurrency").HasMaxLength(3);
        });

        builder.HasIndex(t => t.AccountId);
    }
}
