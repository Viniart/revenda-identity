using Revenda.Identity.Application.Dtos;
using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.Entities;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Application.UseCases.Customers;

public sealed record RegisterCustomerInput(string? Name, string? Cpf, string? Email, string? Password);

public sealed class RegisterCustomerUseCase : IRegisterCustomerUseCase
{
    private readonly ICustomerRepository _customers;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public RegisterCustomerUseCase(
        ICustomerRepository customers,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _customers = customers;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<CustomerOutput> ExecuteAsync(RegisterCustomerInput input, CancellationToken cancellationToken)
    {
        var cpf = Cpf.Create(input.Cpf);
        var email = Email.Create(input.Email);
        var password = Password.Create(input.Password);

        if (await _customers.ExistsByEmailAsync(email, ignoredCustomerId: null, cancellationToken))
        {
            throw DuplicateCustomerException.ForEmail();
        }

        if (await _customers.ExistsByCpfAsync(cpf, cancellationToken))
        {
            throw DuplicateCustomerException.ForCpf();
        }

        var customer = Customer.Register(
            input.Name,
            cpf,
            email,
            _passwordHasher.Hash(password),
            _clock.UtcNow);

        await _customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.CommitAsync(cancellationToken);

        return CustomerOutput.From(customer);
    }
}
