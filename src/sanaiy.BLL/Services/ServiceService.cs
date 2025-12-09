using Microsoft.EntityFrameworkCore;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Service;
using sanaiy.BLL.Entities;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sanaiy.BLL.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResultDto<IEnumerable<ServiceListItemDto>>> GetServicesByCategoryAsync(Guid categoryId)
        {
            try
            {
                var services = await _unitOfWork.Services
                    .GetQueryable()
                    .Where(s => s.CategoryId == categoryId && s.IsActive)
                    .Include(s => s.Category)
                    .Include(s => s.OwnerCraftsman)
                    .OrderBy(s => s.ServiceName)
                    .ToListAsync();

                var serviceDtos = services.Select(service => new ServiceListItemDto
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    IsPriceFixed = service.IsPriceFixed,
                    IsActive = service.IsActive,
                    CategoryId = service.CategoryId,
                    CategoryName = service.Category?.Name ?? "Unknown",
                    CraftsmanId = service.OwnerCraftsmanId,
                    CraftsmanName = $"{service.OwnerCraftsman?.Fname} {service.OwnerCraftsman?.Lname}",
                    CraftsmanRating = 4.5m, // Calculate from Reviews table if available
                    CraftsmanProfileImageUrl = service.OwnerCraftsman?.ProfileImageUrl
                    // مهم جداً

                }).ToList();

                return new ResultDto<IEnumerable<ServiceListItemDto>>
                {
                    Success = true,
                    Data = serviceDtos
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<IEnumerable<ServiceListItemDto>>
                {
                    Success = false,
                    Message = $"Error retrieving services: {ex.Message}"
                };
            }
        }

        public async Task<ResultDto<ServiceDetailsDto>> GetServiceByIdAsync(Guid serviceId)
        {
            try
            {
                var service = await _unitOfWork.Services
                    .GetQueryable()
                    .Include(s => s.Category)
                    .Include(s => s.OwnerCraftsman)
                    .FirstOrDefaultAsync(s => s.Id == serviceId && s.IsActive);

                if (service == null)
                {
                    return new ResultDto<ServiceDetailsDto>
                    {
                        Success = false,
                        Message = "Service not found or not active"
                    };
                }

                var serviceDto = new ServiceDetailsDto
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    IsPriceFixed = service.IsPriceFixed,
                    IsActive = service.IsActive,
                    CreatedAt = service.CreatedAt,
                    CategoryId = service.CategoryId,
                    CategoryName = service.Category?.Name ?? "Unknown",
                    CraftsmanId = service.OwnerCraftsmanId,
                    CraftsmanName = $"{service.OwnerCraftsman?.Fname} {service.OwnerCraftsman?.Lname}",
                    CraftsmanRating = 4.5m,
                    CraftsmanYearsOfExperience = 0,
                    TotalBookings = 0,
                    CompletedBookings = 0,

                };

                return new ResultDto<ServiceDetailsDto>
                {
                    Success = true,
                    Data = serviceDto
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<ServiceDetailsDto>
                {
                    Success = false,
                    Message = $"Error retrieving service: {ex.Message}"
                };
            }
        }

        public async Task<ResultDto<bool>> IsServiceAvailableForBookingAsync(Guid serviceId)
        {
            try
            {
                var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
                var isAvailable = service != null && service.IsActive;

                return new ResultDto<bool>
                {
                    Success = true,
                    Data = isAvailable
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<bool>
                {
                    Success = false,
                    Message = $"Error checking service availability: {ex.Message}"
                };
            }
        }

        public async Task<ResultDto<Guid>> GetCraftsmanIdByServiceAsync(Guid serviceId)
        {
            try
            {
                var service = await _unitOfWork.Services.GetByIdAsync(serviceId);

                if (service == null)
                {
                    return new ResultDto<Guid>
                    {
                        Success = false,
                        Message = "Service not found"
                    };
                }

                return new ResultDto<Guid>
                {
                    Success = true,
                    Data = service.OwnerCraftsmanId
                };
            }
            catch (Exception ex)
            {
                return new ResultDto<Guid>
                {
                    Success = false,
                    Message = $"Error getting craftsman: {ex.Message}"
                };
            }
        }
                public async Task AddServiceAsync(CreateServiceDto dto)
        {
            var newService = new Service
            {
                Id = Guid.NewGuid(),
                ServiceName = dto.ServiceName,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                OwnerCraftsmanId = dto.CraftsmanId,
                IsPriceFixed = dto.IsPriceFixed,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.Services.Add(newService);
            await _unitOfWork.CompleteAsync();
        }
    }
 }
  