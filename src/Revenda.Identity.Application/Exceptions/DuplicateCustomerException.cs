namespace Revenda.Identity.Application.Exceptions;

public sealed class DuplicateCustomerException : ApplicationRuleException
{
    private DuplicateCustomerException(string message) : base(message)
    {
    }

    public static DuplicateCustomerException ForEmail() =>
        new("Já existe um cadastro com este e-mail.");

    public static DuplicateCustomerException ForCpf() =>
        new("Já existe um cadastro com este CPF.");
}
