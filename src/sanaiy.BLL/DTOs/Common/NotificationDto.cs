using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Common
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Info, Success, Warning, Error
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? Link { get; set; } // URL to redirect (e.g., /Booking/Details/5)
    }
}
