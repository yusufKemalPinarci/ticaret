using AutoMapper;
using NotebookTherapy.Application.DTOs;
using NotebookTherapy.Core.Interfaces;

namespace NotebookTherapy.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.Categories.GetActiveCategoriesAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> GetCategoryBySlugAsync(string slug)
    {
        var category = await _unitOfWork.Categories.GetBySlugAsync(slug);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto createDto)
    {
        var category = _mapper.Map<Core.Entities.Category>(createDto);
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto updateDto)
    {
        var existing = await _unitOfWork.Categories.GetByIdAsync(id);
        if (existing == null) return null;
        _mapper.Map(updateDto, existing);
        await _unitOfWork.Categories.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(existing);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var existing = await _unitOfWork.Categories.GetByIdAsync(id);
        if (existing == null) return false;
        existing.IsDeleted = true;
        await _unitOfWork.Categories.UpdateAsync(existing);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}
