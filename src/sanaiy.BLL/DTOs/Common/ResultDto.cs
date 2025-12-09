using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Common
{
    public class ResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();

        // Helper method for quick success
        public static ResultDto SuccessResult(string message = "Operation successful")
        {
            return new ResultDto { Success = true, Message = message };
        }

        // Helper method for quick failure
        public static ResultDto FailureResult(string message, List<string>? errors = null)
        {
            return new ResultDto { Success = false, Message = message, Errors = errors ?? new List<string>() };
        }
    }

    public class ResultDto<T> : ResultDto
    {
        public T? Data { get; set; }

        public static ResultDto<T> SuccessResult(T data, string message = "Operation successful")
        {
            return new ResultDto<T> { Success = true, Message = message, Data = data };
        }

        public static new ResultDto<T> FailureResult(string message, List<string>? errors = null)
        {
            return new ResultDto<T> { Success = false, Message = message, Errors = errors ?? new List<string>() };
        }
    }
}