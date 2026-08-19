using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenda.Identity.Api.Contracts;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.UseCases.Authentication;

namespace Revenda.Identity.Api.Controllers;

[ApiController]
[Route("auth")]
[AllowAnonymous]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticateCustomerUseCase _authenticate;

    public AuthController(IAuthenticateCustomerUseCase authenticate) => _authenticate = authenticate;

    /// <summary>Autentica o comprador e devolve o token usado pelo serviço de veículos.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthenticationOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthenticationOutput>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken) =>
        await _authenticate.ExecuteAsync(
            new AuthenticateCustomerInput(request.Email, request.Password),
            cancellationToken);
}
