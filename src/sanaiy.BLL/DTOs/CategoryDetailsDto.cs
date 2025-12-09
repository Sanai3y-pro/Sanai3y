using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Category
{
    public class CategoryDetailsDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Analytics
        public int ServicesCount { get; set; }
        public int CraftsmenCount { get; set; }
        public int TotalBookings { get; set; }
    }
}