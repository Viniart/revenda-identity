using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Revenda.Identity.Api.Contracts;
using Revenda.Identity.Api.Extensions;
using Revenda.Identity.Application.Dtos;
using Revenda.Identity.Application.Ports.Input;
using Revenda.Identity.Application.UseCases.Customers;

namespace Revenda.Identity.Api.Controllers;

[ApiController]
[Route("customers")]
[Produces("application/json")]
public sealed class CustomersController : ControllerBase
{
    private readonly IRegisterCustomerUseCase _registerCustomer;
    private readonly IGetCustomerProfileUseCase _getProfile;
    private readonly IUpdateCustomerProfileUseCase _updateProfile;

    public CustomersController(
        IRegisterCustomerUseCase registerCustomer,
        IGetCustomerProfileUseCase getProfile,
        IUpdateCustomerProfileUseCase updateProfile)
    {
        _registerCustomer = registerCustomer;
        _getProfile = getProfile;
        _updateProfile = updateProfile;
    }

    /// <summary>Cadastra um comprador. Passo obrigatório antes de qualquer compra.</summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CustomerOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCustomerRequest request,
        CancellationToken cancellationToken)
    {
        var output = await _registerCustomer.ExecuteAsync(
            new RegisterCustomerInput(request.Name, request.Cpf, request.Email, request.Password),
            cancellationToken);

        return CreatedAtAction(nameof(GetProfile), new { }, output);
    }

    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CustomerOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<CustomerOutput>> GetProfile(CancellationToken cancellationToken) =>
        await _getProfile.ExecuteAsync(User.GetCustomerId(), cancellationToken);

    [HttpPut("me")]
    [Authorize]
    [ProducesResponseType(typeof(CustomerOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerOutput>> UpdateProfile(
        [FromBody] UpdateCustomerProfileRequest request,
        CancellationToken cancellationToken) =>
        await _updateProfile.ExecuteAsync(
            new UpdateCustomerProfileInput(User.GetCustomerId(), request.Name, request.Email),
            cancellationToken);
}
