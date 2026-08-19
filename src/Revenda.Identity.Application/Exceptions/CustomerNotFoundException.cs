namespace Revenda.Identity.Application.Exceptions;

public sealed class CustomerNotFoundException : ApplicationRuleException
{
    public CustomerNotFoundException(Guid customerId)
        : base($"Cliente {customerId} não encontrado.")
    {
    }
}
