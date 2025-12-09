using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Review
{
    public class CraftsmanReviewsSummaryDto
    {
        public Guid CraftsmanId { get; set; }
        public string CraftsmanName { get; set; } = string.Empty;

        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        // Star Breakdown (Histogram)
        public int FiveStars { get; set; }
        public int FourStars { get; set; }
        public int ThreeStars { get; set; }
        public int TwoStars { get; set; }
        public int OneStar { get; set; }
    }
}