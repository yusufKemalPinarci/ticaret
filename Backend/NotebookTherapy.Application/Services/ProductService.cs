using AutoMapper;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<IEnumerable<ProductDto>> GetFeaturedProductsAsync()
    {
        var products = await _unitOfWork.Products.GetFeaturedProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetNewProductsAsync()
    {
        var products = await _unitOfWork.Products.GetNewProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetBackInStockProductsAsync()
    {
        var products = await _unitOfWork.Products.GetBackInStockProductsAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId)
    {
        var products = await _unitOfWork.Products.GetProductsByCategoryAsync(categoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCollectionAsync(string collection)
    {
        var products = await _unitOfWork.Products.GetProductsByCollectionAsync(collection);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
    {
        var product = _mapper.Map<Core.Entities.Product>(createDto);
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateDto)
    {
        var existing = await _unitOfWork.Products.GetByIdAsync(id);
        if (existing == null) return null;
        _mapper.Map(updateDto, existing);
        await _unitOfWork.Products.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ProductDto>(existing);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var existing = await _unitOfWork.Products.GetByIdAsync(id);
        if (existing == null) return false;
        existing.IsDeleted = true;
        await _unitOfWork.Products.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
