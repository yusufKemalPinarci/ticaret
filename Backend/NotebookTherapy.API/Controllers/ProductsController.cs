using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotebookTherapy.Application.DTOs;
using MediatR;
using NotebookTherapy.Core.Models;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetAllProductsQuery());
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetProductByIdQuery(id));
        if (product == null)
            return NotFound();
        
        return Ok(product);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeaturedProducts()
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetFeaturedProductsQuery());
        return Ok(products);
    }

    [HttpGet("new")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetNewProducts()
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetNewProductsQuery());
        return Ok(products);
    }

    [HttpGet("back-in-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetBackInStockProducts()
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetBackInStockProductsQuery());
        return Ok(products);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(int categoryId)
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetProductsByCategoryQuery(categoryId));
        return Ok(products);
    }

    [HttpGet("collection/{collection}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCollection(string collection)
    {
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetProductsByCollectionQuery(collection));
        return Ok(products);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> SearchProducts([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search term is required");
        
        var products = await _mediator.Send(new NotebookTherapy.Application.Features.Products.SearchProductsQuery(q));
        return Ok(products);
    }

    [HttpGet("browse")]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> BrowseProducts([FromQuery] ProductFilterOptions filter)
    {
        var result = await _mediator.Send(new NotebookTherapy.Application.Features.Products.GetFilteredProductsQuery(filter));
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createDto)
    {
        var product = await _mediator.Send(new NotebookTherapy.Application.Features.Products.Commands.CreateProductCommand(createDto));
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
    {
        var updated = await _mediator.Send(new NotebookTherapy.Application.Features.Products.Commands.UpdateProductCommand(id, updateDto));
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var deleted = await _mediator.Send(new NotebookTherapy.Application.Features.Products.Commands.DeleteProductCommand(id));
        if (!deleted) return NotFound();
        return NoContent();
    }
}
