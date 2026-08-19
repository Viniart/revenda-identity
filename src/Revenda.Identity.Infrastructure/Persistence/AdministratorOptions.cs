namespace Revenda.Identity.Infrastructure.Persistence;

/// <summary>
/// Credenciais do administrador criado na primeira subida. Só existe porque alguém precisa
/// cadastrar veículos antes de haver qualquer usuário na base.
/// </summary>
public sealed class AdministratorOptions
{
    public const string SectionName = "Administrator";

    public string Name { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Cpf)
        && !string.IsNullOrWhiteSpace(Email)
        && !string.IsNullOrWhiteSpace(Password);
}
