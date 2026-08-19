using Revenda.Identity.Application.Dtos;
using Revenda.Identity.Application.UseCases.Authentication;
using Revenda.Identity.Application.UseCases.Customers;

namespace Revenda.Identity.Application.Ports.Input;

public interface IRegisterCustomerUseCase
{
    Task<CustomerOutput> ExecuteAsync(RegisterCustomerInput input, CancellationToken cancellationToken);
}

public interface IGetCustomerProfileUseCase
{
    Task<CustomerOutput> ExecuteAsync(Guid customerId, CancellationToken cancellationToken);
}

public interface IUpdateCustomerProfileUseCase
{
    Task<CustomerOutput> ExecuteAsync(UpdateCustomerProfileInput input, CancellationToken cancellationToken);
}

public interface IAuthenticateCustomerUseCase
{
    Task<AuthenticationOutput> ExecuteAsync(AuthenticateCustomerInput input, CancellationToken cancellationToken);
}
