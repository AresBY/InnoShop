using InnoShop.Users.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user with the specified credentials.
    /// Accessible to anonymous users. The registered user is assigned the default role (e.g., User).
    /// </summary>
    /// <param name="command">The registration data including name, email, and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 201 Created with the new user ID.</returns>
    [HttpPost(nameof(Register))]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, new { Id = userId });
    }

    /// <summary>
    /// Authenticates a user with provided credentials and returns authentication tokens.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/Login
    ///     {
    ///        "email": "free2107@mail.ru",
    ///        "password": "adminadmin"
    ///     }
    ///
    /// </remarks>
    [HttpPost(nameof(Login))]
    public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Generates a new JWT access token using a valid refresh token.
    /// </summary>
    /// <param name="command">The refresh token command containing the refresh token string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with the new access and refresh tokens.</returns>
    [HttpPost(nameof(Refresh))]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Initiates the password reset process by sending a reset token to the user's email.
    /// </summary>
    /// <param name="command">Contains the user's email to send the reset token to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with a confirmation message.</returns>
    [HttpPost(nameof(ForgotPassword))]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok("Reset token sent to your email.");
    }

    /// <summary>
    /// Resets the user's password using a valid reset token and new password.
    /// </summary>
    /// <param name="command">Contains the reset token and the new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with a success message.</returns>
    [HttpPost(nameof(ResetPassword))]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok("Password successfully reset.");
    }

    /// <summary>
    /// Sends a confirmation email to the user with a verification token.
    /// </summary>
    /// <param name="command">Contains the user's email to send the confirmation to.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with a confirmation message.</returns>
    [HttpPost(nameof(SendEmailConfirmation))]
    public async Task<IActionResult> SendEmailConfirmation([FromBody] SendEmailConfirmationCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return Ok("Confirmation email sent.");
    }

    /// <summary>
    /// Confirms the user's email address using the provided email and confirmation token.
    /// </summary>
    /// <param name="email">The user's email address to confirm.</param>
    /// <param name="token">The confirmation token sent to the user's email.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with a success message.</returns>
    [HttpGet(nameof(ConfirmEmail))]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
    {
        await _mediator.Send(new ConfirmEmailCommand { Email = email, Token = token }, cancellationToken);
        return Ok("Email confirmed successfully.");
    }
}