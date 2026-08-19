namespace Revenda.Identity.Domain.Exceptions;

public sealed class InvalidCustomerDataException : DomainException
{
    public InvalidCustomerDataException(string message) : base(message)
    {
    }
}
