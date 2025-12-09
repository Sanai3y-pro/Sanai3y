using System;
using System.ComponentModel.DataAnnotations;

namespace sanaiy.UI.ViewModels
{
    public class ViolationViewModel
    {
        [Required(ErrorMessage = "Please select a violation type")]
        public string? ViolationType { get; set; }

        [Required(ErrorMessage = "Please enter the involved party")]
        public string? InvolvedParty { get; set; }

        [Required(ErrorMessage = "Please select the date of incident")]
        public DateTime IncidentDate { get; set; }

        public string? Description { get; set; }

        // Optional: يمكن ربط بلاغ بBooking أو TargetUser
        public Guid? TargetBookingId { get; set; }
        public Guid? TargetUserId { get; set; }
    }
}