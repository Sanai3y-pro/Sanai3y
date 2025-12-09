using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Violation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IViolationService
    {
        Task<ResultDto<Guid>> CreateViolationAsync(Guid reporterId, string reporterType, CreateViolationDto createDto);
        Task<ResultDto<ViolationDetailsDto>> GetViolationByIdAsync(Guid violationId);
        Task<ResultDto<IEnumerable<ViolationListItemDto>>> GetAllViolationsAsync();
        Task<ResultDto<IEnumerable<ViolationListItemDto>>> GetPendingViolationsAsync();
        Task<ResultDto> UpdateViolationStatusAsync(UpdateViolationStatusDto updateDto);
    }
}
