using Revenda.Identity.Application.Dtos;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Application.UseCases.Customers;

public sealed record UpdateCustomerProfileInput(Guid CustomerId, string? Name, string? Email);

public sealed class UpdateCustomerProfileUseCase : IUpdateCustomerProfileUseCase
{
    private readonly ICustomerRepository _customers;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public UpdateCustomerProfileUseCase(ICustomerRepository customers, IUnitOfWork unitOfWork, IClock clock)
    {
        _customers = customers;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<CustomerOutput> ExecuteAsync(
        UpdateCustomerProfileInput input,
        CancellationToken cancellationToken)
    {
        var customer = await _customers.FindByIdAsync(input.CustomerId, cancellationToken)
            ?? throw new CustomerNotFoundException(input.CustomerId);

        var email = Email.Create(input.Email);

        if (await _customers.ExistsByEmailAsync(email, customer.Id, cancellationToken))
        {
            throw DuplicateCustomerException.ForEmail();
        }

        customer.ChangeProfile(input.Name, email, _clock.UtcNow);
        await _unitOfWork.CommitAsync(cancellationToken);

        return CustomerOutput.From(customer);
    }
}
