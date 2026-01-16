using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Categories.Handlers;

public class CategoryQueryHandlers :
    IRequestHandler<NotebookTherapy.Application.Features.Categories.GetAllCategoriesQuery, IEnumerable<CategoryDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Categories.GetCategoryByIdQuery, CategoryDto?>,
    IRequestHandler<NotebookTherapy.Application.Features.Categories.GetCategoryBySlugQuery, CategoryDto?>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllCategoriesKey = "categories_all";

    public CategoryQueryHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(NotebookTherapy.Application.Features.Categories.GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AllCategoriesKey, out IEnumerable<CategoryDto> cached))
            return cached;

        var categories = await _uow.Categories.GetActiveCategoriesAsync();
        var dtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        _cache.Set(AllCategoriesKey, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dtos;
    }

    public async Task<CategoryDto?> Handle(NotebookTherapy.Application.Features.Categories.GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var key = $"category_{request.Id}";
        if (_cache.TryGetValue(key, out CategoryDto cached))
            return cached;

        var category = await _uow.Categories.GetByIdAsync(request.Id);
        if (category == null) return null;
        var dto = _mapper.Map<CategoryDto>(category);
        _cache.Set(key, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dto;
    }

    public async Task<CategoryDto?> Handle(NotebookTherapy.Application.Features.Categories.GetCategoryBySlugQuery request, CancellationToken cancellationToken)
    {
        var key = $"category_slug_{request.Slug}";
        if (_cache.TryGetValue(key, out CategoryDto cached))
            return cached;

        var category = await _uow.Categories.GetBySlugAsync(request.Slug);
        if (category == null) return null;
        var dto = _mapper.Map<CategoryDto>(category);
        _cache.Set(key, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dto;
    }
}
