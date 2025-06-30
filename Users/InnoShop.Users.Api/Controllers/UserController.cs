using InnoShop.Users.Application.Features.Users.Commands;
using InnoShop.Users.Application.Features.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Users.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves a list of all users.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with a list of users.</returns>
    [HttpGet(nameof(GetAll))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var users = await _mediator.Send(new GetAllUsersQuery(), cancellationToken);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves detailed information about a specific user by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 200 OK with user details if found; otherwise, 404 Not Found.</returns>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _mediator.Send(new GetUserByIdQuery() { Id = id }, cancellationToken);
        if (user == null)
            return NotFound();

        return Ok(user);
    }

    /// <summary>
    /// Creates a new user with the specified data.
    /// </summary>
    /// <param name="command">The command containing user creation data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// Returns 201 Created with a location header pointing to the newly created user.
    /// </returns>
    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = userId }, null);
    }

    /// <summary>
    /// Partially updates a user's data.
    /// </summary>
    /// <param name="command">Fields to update, including user ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content if successful; 404 if user not found.</returns>
    [HttpPatch(Name = nameof(Update))]
    public async Task<IActionResult> Update([FromBody] UpdateUserCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Returns 204 No Content on successful deletion.</returns>
    [HttpDelete("{id:guid}", Name = nameof(Delete))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteUserCommand() { Id = id }, cancellationToken);
        return NoContent();
    }
}
