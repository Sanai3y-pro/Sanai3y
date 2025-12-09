using sanaiy.BLL.DTOs.Category;
using sanaiy.BLL.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface ICategoryService
    {
        // ONLY KEEP THESE TWO FOR NOW:
        Task<ResultDto<CategoryDetailsDto>> GetCategoryByIdAsync(Guid categoryId);
        Task<ResultDto<IEnumerable<CategoryListItemDto>>> GetAllCategoriesAsync();

    }
}