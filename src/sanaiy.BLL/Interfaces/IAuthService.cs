using sanaiy.BLL.DTOs.Auth;
using sanaiy.BLL.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IAuthService
    {
        Task<ResultDto<LoginResponseDto>> LoginClientAsync(LoginDto loginDto);
        Task<ResultDto<LoginResponseDto>> LoginCraftsmanAsync(LoginDto loginDto);
        Task<ResultDto<Guid>> RegisterClientAsync(RegisterClientDto registerDto);
        Task<ResultDto<Guid>> RegisterCraftsmanAsync(RegisterCraftsmanDto registerDto);
        Task<ResultDto> ChangePasswordAsync(Guid userId, string userType, string oldPassword, string newPassword);
        Task<ResultDto> ResetPasswordAsync(string email, string userType);
    }
}
