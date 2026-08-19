using Revenda.Identity.Application.Dtos;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.Ports.Output;

namespace Revenda.Identity.Application.UseCases.Customers;

public sealed class GetCustomerProfileUseCase : IGetCustomerProfileUseCase
{
    private readonly ICustomerRepository _customers;

    public GetCustomerProfileUseCase(ICustomerRepository customers) => _customers = customers;

    public async Task<CustomerOutput> ExecuteAsync(Guid customerId, CancellationToken cancellationToken)
    {
        var customer = await _customers.FindByIdAsync(customerId, cancellationToken)
            ?? throw new CustomerNotFoundException(customerId);

        return CustomerOutput.From(customer);
    }
}
