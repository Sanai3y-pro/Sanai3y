using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Common
{
    public class SearchFilterDto
    {
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public string? City { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinRating { get; set; } // 1 to 5

        public string? SortBy { get; set; } // e.g., "rating", "price", "date"
        public bool Ascending { get; set; } = true;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}