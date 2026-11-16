using BankSim.Application.Dtos;
using FluentValidation;

namespace BankSim.Application.Validation;

public sealed class CreateCustomerRequestValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(r => r.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.LastName).NotEmpty().MaximumLength(100);
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.DateOfBirth)
            .LessThan(_ => DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth must be in the past.");
    }
}

public sealed class OpenAccountRequestValidator : AbstractValidator<OpenAccountRequest>
{
    public OpenAccountRequestValidator()
    {
        RuleFor(r => r.CustomerId).NotEmpty();
        RuleFor(r => r.InitialDeposit).GreaterThanOrEqualTo(0);
        RuleFor(r => r.Currency).NotEmpty().Length(3);
        RuleFor(r => r.Type).IsInEnum();
    }
}

public sealed class DepositRequestValidator : AbstractValidator<DepositRequest>
{
    public DepositRequestValidator() => RuleFor(r => r.Amount).GreaterThan(0);
}

public sealed class WithdrawRequestValidator : AbstractValidator<WithdrawRequest>
{
    public WithdrawRequestValidator() => RuleFor(r => r.Amount).GreaterThan(0);
}

public sealed class TransferRequestValidator : AbstractValidator<TransferRequest>
{
    public TransferRequestValidator()
    {
        RuleFor(r => r.FromAccountId).NotEmpty();
        RuleFor(r => r.ToAccountId).NotEmpty();
        RuleFor(r => r.Amount).GreaterThan(0);
        RuleFor(r => r).Must(r => r.FromAccountId != r.ToAccountId)
            .WithMessage("Cannot transfer to the same account.");
    }
}
