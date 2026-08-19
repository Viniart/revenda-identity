using System.ComponentModel.DataAnnotations;

namespace Revenda.Identity.Api.Contracts;

public sealed record RegisterCustomerRequest(
    [Required][StringLength(150, MinimumLength = 2)] string Name,
    [Required][StringLength(14, MinimumLength = 11)] string Cpf,
    [Required][EmailAddress][StringLength(254)] string Email,
    [Required][StringLength(72, MinimumLength = 8)] string Password);

public sealed record UpdateCustomerProfileRequest(
    [Required][StringLength(150, MinimumLength = 2)] string Name,
    [Required][EmailAddress][StringLength(254)] string Email);

public sealed record LoginRequest(
    [Required] string Email,
    [Required] string Password);
