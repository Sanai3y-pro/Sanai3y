using Microsoft.EntityFrameworkCore;
using sanaiy.BLL.DTOs.Category;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // ONLY IMPLEMENT WHAT YOU NEED FOR DISPLAY
        public async Task<ResultDto<IEnumerable<CategoryListItemDto>>> GetAllCategoriesAsync()
        {
            try
            {
                // Get queryable from repository
                var query = _unitOfWork.Categories.GetQueryable();

                // Get active categories
                var categories = await query
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                // Map to DTOs
                var categoryDtos = categories.Select(c => new CategoryListItemDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ImageUrl = c.Image
                    // Your DTO doesn't have Image property, so don't include it
                }).ToList();

                return new ResultDto<IEnumerable<CategoryListItemDto>>
                {
                    Success = true,
                    Data = categoryDtos
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<CategoryListItemDto>>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // FOR LATER - when you click a category to see its services
        public async Task<ResultDto<CategoryDetailsDto>> GetCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                var category = await _unitOfWork.Categories.GetByIdAsync(categoryId);

                if (category == null || !category.IsActive)
                {
                    return new ResultDto<CategoryDetailsDto>
                    {
                        Success = false,
                        Message = "Category not found"
                    };
                }

                // Map to DTO
                var categoryDto = new CategoryDetailsDto
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    ImageUrl = category.Image, // This will be null if no image
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    ServicesCount = 0 // We'll calculate this later
                };

                return new ResultDto<CategoryDetailsDto>
                {
                    Success = true,
                    Data = categoryDto
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<CategoryDetailsDto>
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }

        // Remove or comment out all other methods for now
        public Task<ResultDto<IEnumerable<CategoryListItemDto>>> GetActiveCategoriesAsync()
        {
            // Just call GetAllCategoriesAsync since both return active categories
            return GetAllCategoriesAsync();
        }

    }
}