namespace BankSim.Domain.Exceptions;

/// <summary>Base type for every business-rule violation raised by the domain layer.</summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message) { }
}
