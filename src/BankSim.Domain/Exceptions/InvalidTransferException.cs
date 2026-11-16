namespace BankSim.Domain.Exceptions;

public sealed class InvalidTransferException : DomainException
{
    public InvalidTransferException(string message) : base(message) { }

    public static InvalidTransferException SameAccount(Guid accountId) =>
        new($"Cannot transfer from account '{accountId}' to itself.");

    public static InvalidTransferException NonPositiveAmount() =>
        new("Transfer amount must be greater than zero.");
}
