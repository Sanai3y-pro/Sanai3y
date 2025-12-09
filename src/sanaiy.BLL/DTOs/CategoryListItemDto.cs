using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Category
{
    public class CategoryListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; } // URL for display
        public bool IsActive { get; set; }

        // Stats for UI Badges
        public int ServicesCount { get; set; }
        public int CraftsmenCount { get; set; }
    }
}