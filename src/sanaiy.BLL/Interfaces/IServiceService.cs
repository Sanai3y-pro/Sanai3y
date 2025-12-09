using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IServiceService
    {
        // Core methods for service display and selection
        Task<ResultDto<ServiceDetailsDto>> GetServiceByIdAsync(Guid serviceId);
        Task<ResultDto<IEnumerable<ServiceListItemDto>>> GetServicesByCategoryAsync(Guid categoryId);

        // For service availability check
        Task<ResultDto<bool>> IsServiceAvailableForBookingAsync(Guid serviceId);

        // For craftsman details (if needed in booking)
        Task<ResultDto<Guid>> GetCraftsmanIdByServiceAsync(Guid serviceId);
        Task AddServiceAsync(CreateServiceDto dto);
    }
}