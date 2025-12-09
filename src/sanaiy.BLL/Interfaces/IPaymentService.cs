using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.DTOs.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IPaymentService
    {
        Task<ResultDto<ProcessPaymentResponseDto>> ProcessPaymentAsync(CreatePaymentDto paymentDto);
        Task<ResultDto<PaymentDetailsDto>> GetPaymentByIdAsync(Guid paymentId);
        Task<ResultDto> UpdatePaymentStatusAsync(UpdatePaymentStatusDto updateDto);
        Task<ResultDto> RefundPaymentAsync(Guid paymentId, string reason);
    }
}