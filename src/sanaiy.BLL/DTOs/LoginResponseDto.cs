using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Auth
{
    public class LoginResponseDto
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty; // "Client" or "Craftsman"
        public bool IsVerified { get; set; } // Important for Craftsman logic
        public string? ProfileImageUrl { get; set; }
    }
}