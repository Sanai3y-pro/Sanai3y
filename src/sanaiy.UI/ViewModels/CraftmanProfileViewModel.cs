using sanaiy.BLL.DTOs;
using sanaiy.BLL.DTOs.Category;
using sanaiy.BLL.Entities;
using System;
using System.Collections.Generic;

namespace sanaiy.UI.ViewModels
{
    public class CraftsmanServiceViewModel
    {
        public Guid ServiceId { get; set; }
        public string? Name { get; set; }
        public string? ShortDescription { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public bool IsAdded { get; set; }
    }

    public class AvailabilityViewModel
    {
        public string Day { get; set; } = "";
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
    }

    public class ReviewViewModel
    {
        public string ClientName { get; set; } = "";
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CraftsmanProfileViewModel
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }
        public string? ProfessionalTitle { get; set; }
        public string? Bio { get; set; }

        public string? Location { get; set; }
        public string LocationLabel => string.IsNullOrEmpty(Location) ? "N/A" : Location;

        public string? ProfileImageUrl { get; set; }

        public string? Phone { get; set; }
        public string? Email { get; set; }

        public decimal RatingAverage { get; set; }
        public int TotalReviews { get; set; }
        public string RatingLabel => $"{RatingAverage} / 5.0 · {TotalReviews} reviews";

        public int TotalBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int PendingBookings { get; set; }

        // FIXED LIST TYPES
        public List<CraftsmanServiceViewModel> ServicesOffered { get; set; } = new();
        public List<BookingViewModel> BookedServices { get; set; } = new();
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public List<AvailabilityViewModel> Availability { get; set; } = new();
        public List<CategoryListItemDto> Categories { get; set; } = new List<CategoryListItemDto>();
        public string? City { get; set; }
        public Guid CategoryId { get; set; }
    }
}
