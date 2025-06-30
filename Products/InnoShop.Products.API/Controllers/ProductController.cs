using InnoShop.Products.Application.Features.Products.Commands;
using InnoShop.Products.Application.Features.Products.Queries;
using InnoShop.Shared.DTOs.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InnoShop.Products.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all products, optionally filtering by visibility.
    /// </summary>
    /// <param name="includeInvisible">If true, includes invisible products. Default is false.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of products.</returns>
    [HttpGet(nameof(GetAll))]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInvisible = false, CancellationToken cancellationToken = default)
    {
        var query = new GetAllProductsQuery(includeInvisible);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }


    /// <summary>
    /// Searches products based on query parameters.
    /// </summary>
    /// <param name="query">Search parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of matching products.</returns>
    [HttpGet(nameof(Search))]
    public async Task<IActionResult> Search([FromQuery] SearchProductsQuery query, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">Product ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The product details.</returns>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    /// <summary>
    /// Creates a new product. Requires authentication.
    /// </summary>
    /// <param name="command">The data for the new product.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The ID of the created product.</returns>
    [Authorize]
    [HttpPost(nameof(Create))]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    /// <summary>
    /// Updates an existing product. Requires authentication and ownership.
    /// </summary>
    /// <param name="id">The product ID from the route.</param>
    /// <param name="command">Updated product data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [Authorize]
    [HttpPut("{id:guid}", Name = nameof(Update))]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a product. Requires authentication and ownership.
    /// </summary>
    /// <param name="id">The ID of the product to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    [Authorize]
    [HttpDelete("{id:guid}", Name = nameof(Delete))]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Updates visibility of products owned by a specific user.
    /// </summary>
    /// <param name="dto">User status change info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>204 No Content on success.</returns>
    [HttpPost(Name = nameof(UpdateProductsVisibility))]
    public async Task<IActionResult> UpdateProductsVisibility([FromBody] UserStatusChanged dto, CancellationToken cancellationToken)
    {
        if (dto == null)
            return BadRequest("User status data is required.");

        await _mediator.Send(new UpdateProductsVisibilityCommand(dto.UserId, dto.IsActive), cancellationToken);
        return NoContent();
    }
}
