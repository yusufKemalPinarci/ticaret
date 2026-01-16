using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;
using NotebookTherapy.Application.Features.Products.Commands;
using System.Threading;
using System.Threading.Tasks;

namespace NotebookTherapy.Application.Features.Products.Handlers;

public class ProductCommandHandlers :
    IRequestHandler<CreateProductCommand, ProductDto>,
    IRequestHandler<UpdateProductCommand, ProductDto?>,
    IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private const string AllProductsKey = "products_all";

    public ProductCommandHandlers(IUnitOfWork uow, IMapper mapper, IMemoryCache cache)
    {
        _uow = uow;
        _mapper = mapper;
        _cache = cache;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = _mapper.Map<Core.Entities.Product>(request.CreateDto);
        await _uow.Products.AddAsync(product);
        await _uow.CommitAsync();
        _cache.Remove(AllProductsKey);
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdAsync(request.Id);
        if (product == null) return null;
        _mapper.Map(request.UpdateDto, product);
        await _uow.Products.UpdateAsync(product);
        await _uow.CommitAsync();
        _cache.Remove(AllProductsKey);
        _cache.Remove($"product_{request.Id}");
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdAsync(request.Id);
        if (product == null) return false;
        await _uow.Products.DeleteAsync(product);
        await _uow.CommitAsync();
        _cache.Remove(AllProductsKey);
        _cache.Remove($"product_{request.Id}");
        return true;
    }
}
