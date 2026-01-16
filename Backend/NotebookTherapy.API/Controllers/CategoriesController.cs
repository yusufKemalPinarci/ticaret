using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotebookTherapy.Application.DTOs;
using MediatR;

namespace NotebookTherapy.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAllCategories()
    {
        var categories = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.GetAllCategoriesQuery());
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.GetCategoryByIdQuery(id));
        if (category == null)
            return NotFound();
        
        return Ok(category);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryBySlug(string slug)
    {
        var category = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.GetCategoryBySlugQuery(slug));
        if (category == null)
            return NotFound();
        
        return Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto createDto)
    {
        var category = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.Commands.CreateCategoryCommand(createDto));
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, [FromBody] UpdateCategoryDto updateDto)
    {
        var updated = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.Commands.UpdateCategoryCommand(id, updateDto));
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        var deleted = await _mediator.Send(new NotebookTherapy.Application.Features.Categories.Commands.DeleteCategoryCommand(id));
        if (!deleted) return NotFound();
        return NoContent();
    }
}
