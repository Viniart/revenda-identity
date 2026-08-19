using Microsoft.Extensions.DependencyInjection;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.UseCases.Authentication;
using Revenda.Identity.Application.UseCases.Customers;

namespace Revenda.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityApplication(this IServiceCollection services)
    {
        services.AddScoped<IRegisterCustomerUseCase, RegisterCustomerUseCase>();
        services.AddScoped<IGetCustomerProfileUseCase, GetCustomerProfileUseCase>();
        services.AddScoped<IUpdateCustomerProfileUseCase, UpdateCustomerProfileUseCase>();
        services.AddScoped<IAuthenticateCustomerUseCase, AuthenticateCustomerUseCase>();

        return services;
    }
}
