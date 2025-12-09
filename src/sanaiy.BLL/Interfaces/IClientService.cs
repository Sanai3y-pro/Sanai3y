using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Client;
using sanaiy.BLL.DTOs.Address;

namespace sanaiy.BLL.Interfaces
{
    public interface IClientService
    {
        // الدوال الأساسية الموجودة
        Task<ResultDto<ClientProfileDto>> GetProfileAsync(Guid clientId);
        Task<ResultDto> UpdateProfileAsync(Guid clientId, UpdateClientProfileDto updateDto);
        Task<ResultDto<IEnumerable<ClientListItemDto>>> GetAllClientsAsync();
        Task<ResultDto<PaginatedResultDto<ClientListItemDto>>> GetClientsPaginatedAsync(int pageNumber, int pageSize);
        Task<ResultDto> DeactivateClientAsync(Guid clientId);
        Task<ResultDto> ActivateClientAsync(Guid clientId);
        Task<ResultDto> ChangePasswordAsync(Guid clientId, string currentPassword, string newPassword);
        Task<ResultDto> DeleteAccountAsync(Guid clientId, string password);

        // أضيفي هذه الدوال الجديدة
        //Task<ResultDto<IEnumerable<AddressListItemDto>>> GetClientAddressesAsync(Guid clientId);
        //Task<ResultDto<AddressListItemDto>> GetAddressByIdAsync(Guid clientId, Guid addressId);
        //Task<ResultDto<Guid>> AddAddressAsync(Guid clientId, CreateAddressDto createDto);
        //Task<ResultDto> UpdateAddressAsync(Guid clientId, Guid addressId, CreateAddressDto updateDto);
        //Task<ResultDto> DeleteAddressAsync(Guid clientId, Guid addressId);
        //Task<ResultDto<AddressListItemDto>> GetDefaultAddressAsync(Guid clientId);
    }
}