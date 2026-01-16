using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Products.Handlers;

public class ProductQueryHandlers :
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetAllProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetProductByIdQuery, ProductDto?>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetFeaturedProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetNewProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetBackInStockProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetProductsByCategoryQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetProductsByCollectionQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.SearchProductsQuery, IEnumerable<ProductDto>>,
    IRequestHandler<NotebookTherapy.Application.Features.Products.GetFilteredProductsQuery, PagedResultDto<ProductDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllProductsKey = "products_all";

    public ProductQueryHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(AllProductsKey, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetAllAsync();
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(AllProductsKey, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<ProductDto?> Handle(NotebookTherapy.Application.Features.Products.GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"product_{request.Id}";
        if (_cache.TryGetValue(cacheKey, out ProductDto cached))
            return cached;

        var product = await _uow.Products.GetByIdAsync(request.Id);
        if (product == null) return null;
        var dto = _mapper.Map<ProductDto>(product);
        _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(10) });
        return dto;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetFeaturedProductsQuery request, CancellationToken cancellationToken)
    {
        const string key = "products_featured";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetFeaturedProductsAsync();
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetNewProductsQuery request, CancellationToken cancellationToken)
    {
        const string key = "products_new";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetNewProductsAsync();
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetBackInStockProductsQuery request, CancellationToken cancellationToken)
    {
        const string key = "products_backinstock";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetBackInStockProductsAsync();
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var key = $"products_category_{request.CategoryId}";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetProductsByCategoryAsync(request.CategoryId);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetProductsByCollectionQuery request, CancellationToken cancellationToken)
    {
        var key = $"products_collection_{request.Collection}";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.GetProductsByCollectionAsync(request.Collection);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) });
        return dtos;
    }

    public async Task<IEnumerable<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var key = $"products_search_{request.SearchTerm}";
        if (_cache.TryGetValue(key, out IEnumerable<ProductDto> cached))
            return cached;

        var products = await _uow.Products.SearchProductsAsync(request.SearchTerm);
        var dtos = _mapper.Map<IEnumerable<ProductDto>>(products);
        _cache.Set(key, dtos, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(2) });
        return dtos;
    }

    public async Task<PagedResultDto<ProductDto>> Handle(NotebookTherapy.Application.Features.Products.GetFilteredProductsQuery request, CancellationToken cancellationToken)
    {
        var result = await _uow.Products.GetFilteredProductsAsync(request.Options);
        return new PagedResultDto<ProductDto>
        {
            Items = _mapper.Map<IReadOnlyList<ProductDto>>(result.Items),
            Total = result.Total,
            Page = result.Page,
            PageSize = result.PageSize
        };
    }
}
