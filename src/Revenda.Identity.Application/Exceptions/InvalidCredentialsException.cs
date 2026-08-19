namespace Revenda.Identity.Application.Exceptions;

public sealed class InvalidCredentialsException : ApplicationRuleException
{
    public InvalidCredentialsException() : base("E-mail ou senha inválidos.")
    {
    }
}
