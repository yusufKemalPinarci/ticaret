using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Features.Categories.Commands;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace NotebookTherapy.Application.Features.Categories.Handlers;

public class CategoryCommandHandlers :
    IRequestHandler<CreateCategoryCommand, CategoryDto>,
    IRequestHandler<UpdateCategoryCommand, CategoryDto?>,
    IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllCategoriesKey = "categories_all";

    public CategoryCommandHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = _mapper.Map<Core.Entities.Category>(request.CreateDto);
        category.Slug = await GenerateUniqueSlugAsync(request.CreateDto.Slug, request.CreateDto.Name, null);
        await _uow.Categories.AddAsync(category);
        await _uow.CommitAsync();
        _cache.Remove(AllCategoriesKey);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id);
        if (category == null) return null;
        _mapper.Map(request.UpdateDto, category);
        category.Slug = await GenerateUniqueSlugAsync(request.UpdateDto.Slug, request.UpdateDto.Name, request.Id);
        await _uow.Categories.UpdateAsync(category);
        await _uow.CommitAsync();
        _cache.Remove(AllCategoriesKey);
        _cache.Remove($"category_{request.Id}");
        _cache.Remove($"category_slug_{category.Slug}");
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _uow.Categories.GetByIdAsync(request.Id);
        if (category == null) return false;
        await _uow.Categories.DeleteAsync(category);
        await _uow.CommitAsync();
        _cache.Remove(AllCategoriesKey);
        _cache.Remove($"category_{request.Id}");
        _cache.Remove($"category_slug_{category.Slug}");
        return true;
    }

    private async Task<string> GenerateUniqueSlugAsync(string requestedSlug, string? nameFallback, int? excludeCategoryId)
    {
        var baseSlug = Slugify(string.IsNullOrWhiteSpace(requestedSlug) ? nameFallback : requestedSlug);
        if (string.IsNullOrWhiteSpace(baseSlug))
        {
            baseSlug = "category";
        }

        var uniqueSlug = baseSlug;
        var suffix = 1;
        while (await _uow.Categories.SlugExistsAsync(uniqueSlug, excludeCategoryId))
        {
            uniqueSlug = $"{baseSlug}-{suffix}";
            suffix++;
        }

        return uniqueSlug;
    }

    private static string Slugify(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;

        var slug = value
            .Trim()
            .ToLowerInvariant();

        slug = Regex.Replace(slug, "[^a-z0-9]+", "-");
        slug = slug.Trim('-');
        return slug;
    }
}
