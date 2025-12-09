using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Common
{
    public class DashboardStatisticsDto
    {
        // User Stats
        public int TotalClients { get; set; }
        public int TotalCraftsmen { get; set; }

        // Booking Stats
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CompletedBookings { get; set; }

        // Financial Stats
        public decimal TotalRevenue { get; set; }
        public decimal TotalCommissions { get; set; }

        // Quality Stats
        public int TotalReviews { get; set; }
        public decimal AverageRating { get; set; }
    }
}
