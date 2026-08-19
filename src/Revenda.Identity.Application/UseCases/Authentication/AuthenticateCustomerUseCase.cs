using Revenda.Identity.Application.Exceptions;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.Ports.Output;
using Revenda.Identity.Domain.ValueObjects;

namespace Revenda.Identity.Application.UseCases.Authentication;

public sealed record AuthenticateCustomerInput(string? Email, string? Password);

public sealed record AuthenticationOutput(string AccessToken, string TokenType, long ExpiresIn);

public sealed class AuthenticateCustomerUseCase : IAuthenticateCustomerUseCase
{
    private const string DummyHash =
        "AQAAAAIAAYagAAAAEHxV1sBpZ0Ic8u1JhKUdEuUj0k2rE0FhFf7BSm5oXR0kQeSwsMzYy0dQm/8dPWzEUw==";

    private readonly ICustomerRepository _customers;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenIssuer _tokenIssuer;
    private readonly IClock _clock;

    public AuthenticateCustomerUseCase(
        ICustomerRepository customers,
        IPasswordHasher passwordHasher,
        IAccessTokenIssuer tokenIssuer,
        IClock clock)
    {
        _customers = customers;
        _passwordHasher = passwordHasher;
        _tokenIssuer = tokenIssuer;
        _clock = clock;
    }

    public async Task<AuthenticationOutput> ExecuteAsync(
        AuthenticateCustomerInput input,
        CancellationToken cancellationToken)
    {
        if (!Email.TryCreate(input.Email, out var email))
        {
            throw new InvalidCredentialsException();
        }

        var customer = await _customers.FindByEmailAsync(email, cancellationToken);

        // Verifica o hash mesmo sem cadastro correspondente para que o tempo de resposta
        // não revele quais e-mails existem na base.
        var passwordMatches = _passwordHasher.Verify(
            customer?.PasswordHash ?? DummyHash,
            input.Password ?? string.Empty);

        if (customer is null || !passwordMatches)
        {
            throw new InvalidCredentialsException();
        }

        var token = _tokenIssuer.Issue(customer);
        var expiresIn = (long)(token.ExpiresAt - _clock.UtcNow).TotalSeconds;

        return new AuthenticationOutput(token.Value, "Bearer", expiresIn);
    }
}
