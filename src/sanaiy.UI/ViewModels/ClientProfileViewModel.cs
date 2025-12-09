using sanaiy.BLL.DTOs.Review;
using System;
using System.Collections.Generic;

namespace sanaiy.UI.ViewModels
{
    public class ClientProfileViewModel
    {
        // BASIC INFO
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? ProfileImageUrl { get; set; }

        public string? Location { get; set; }
        public string LocationLabel => string.IsNullOrEmpty(Location) ? "N/A" : Location;

        // STATISTICS
        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }

        // LISTS
        public List<BookingViewModel> BookedServices { get; set; } = new();
        public List<ReviewListItemDto> Reviews { get; set; } = new();

    }
}
