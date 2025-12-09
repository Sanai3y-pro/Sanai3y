using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IServiceManagementService
    {
        Task<ResultDto<ServiceDetailsDto>> GetServiceByIdAsync(Guid serviceId);
        Task<ResultDto<IEnumerable<ServiceListItemDto>>> GetAllServicesAsync();
        Task<ResultDto<IEnumerable<ServiceListItemDto>>> GetServicesByCategoryAsync(Guid categoryId);
        Task<ResultDto<IEnumerable<ServiceListItemDto>>> GetServicesByCraftsmanAsync(Guid craftsmanId);
        Task<ResultDto<Guid>> CreateServiceAsync(Guid craftsmanId, CreateServiceDto createDto);
        Task<ResultDto> UpdateServiceAsync(Guid craftsmanId, UpdateServiceDto updateDto);
        Task<ResultDto> DeleteServiceAsync(Guid craftsmanId, Guid serviceId);
        Task<ResultDto<PaginatedResultDto<ServiceListItemDto>>> SearchServicesAsync(SearchFilterDto filter);
    }
}