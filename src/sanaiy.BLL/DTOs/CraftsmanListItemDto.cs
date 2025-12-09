using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Craftsman
{
    public class CraftsmanListItemDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int? YearsOfExperience { get; set; }
        public decimal RatingAverage { get; set; }
        public int TotalReviews { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Bio { get; set; }
        public bool IsVerified { get; set; }
    }
}