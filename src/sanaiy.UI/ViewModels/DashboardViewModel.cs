using System.Collections.Generic;

namespace sanaiy.UI.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCraftsmen { get; set; }
        public int TotalServices { get; set; }
        public int TotalBookings { get; set; }
        public int TotalClients { get; set; }

        public IEnumerable<BookingViewModel> RecentBookings { get; set; } = new List<BookingViewModel>();
    }
}