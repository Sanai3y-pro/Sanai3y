using sanaiy.BLL.DTOs.Address;
using sanaiy.BLL.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IAddressService
    {
        Task<ResultDto<Guid>> CreateAddressAsync(Guid userId, string userType, CreateAddressDto createDto);
        Task<ResultDto<IEnumerable<AddressListItemDto>>> GetUserAddressesAsync(Guid userId, string userType);
        Task<ResultDto> UpdateAddressAsync(Guid userId, UpdateAddressDto updateDto);
        Task<ResultDto> DeleteAddressAsync(Guid addressId, Guid userId);
        Task<ResultDto> SetDefaultAddressAsync(Guid addressId, Guid userId);
        Task<ResultDto<AddressListItemDto>> GetAddressByIdAsync(Guid addressId);

    }
}