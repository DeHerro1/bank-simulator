using BankSim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankSim.Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(320);
        builder.Property(c => c.DateOfBirth).IsRequired();

        builder.HasIndex(c => c.Email).IsUnique();

        // Customer.Accounts is a read-only projection over the private `_accounts` list — EF Core's
        // backing-field convention finds `_accounts` automatically and writes through it on load.
        builder.HasMany(c => c.Accounts)
            .WithOne()
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
