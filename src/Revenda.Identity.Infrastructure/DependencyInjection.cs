using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Infrastructure.Persistence;
using Revenda.Identity.Infrastructure.Persistence.Context;
using Revenda.Identity.Infrastructure.Persistence.Repositories;
using Revenda.Identity.Infrastructure.Security;
using Revenda.Identity.Infrastructure.Time;

namespace Revenda.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                npgsql => npgsql.MigrationsHistoryTable("__migrations", "identity")));

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AdministratorOptions>(configuration.GetSection(AdministratorOptions.SectionName));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DatabaseBootstrapper>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<SigningKeyProvider>();
        services.AddSingleton<IJsonWebKeySetProvider>(provider => provider.GetRequiredService<SigningKeyProvider>());
        services.AddSingleton<IAccessTokenIssuer, JwtAccessTokenIssuer>();

        return services;
    }
}
