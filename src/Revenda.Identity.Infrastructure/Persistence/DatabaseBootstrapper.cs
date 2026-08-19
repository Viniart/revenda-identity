using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;
using Revenda.Identity.Infrastructure.Persistence.Context;

namespace Revenda.Identity.Infrastructure.Persistence;

public sealed class DatabaseBootstrapper
{
    private readonly IdentityDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IClock _clock;
    private readonly AdministratorOptions _administrator;
    private readonly ILogger<DatabaseBootstrapper> _logger;

    public DatabaseBootstrapper(
        IdentityDbContext context,
        IPasswordHasher passwordHasher,
        IClock clock,
        IOptions<AdministratorOptions> administrator,
        ILogger<DatabaseBootstrapper> logger)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _clock = clock;
        _administrator = administrator.Value;
        _logger = logger;
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await _context.Database.MigrateAsync(cancellationToken);
        await EnsureAdministratorAsync(cancellationToken);
    }

    private async Task EnsureAdministratorAsync(CancellationToken cancellationToken)
    {
        if (!_administrator.IsConfigured)
        {
            _logger.LogWarning("Administrador não configurado; nenhum usuário administrativo foi criado.");
            return;
        }

        var email = Email.Create(_administrator.Email);

        if (await _context.Customers.AnyAsync(customer => customer.Email == email, cancellationToken))
        {
            return;
        }

        var admin = Customer.RegisterAdministrator(
            _administrator.Name,
            Cpf.Create(_administrator.Cpf),
            email,
            _passwordHasher.Hash(Password.Create(_administrator.Password)),
            _clock.UtcNow);

        _context.Customers.Add(admin);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Administrador {CustomerId} criado.", admin.Id);
    }
}
