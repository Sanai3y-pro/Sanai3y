using System.ComponentModel.DataAnnotations;

namespace sanaiy.UI.ViewModels
{
    public class SettingViewModel
    {
        // Change Password Section
        [Required(ErrorMessage = "Current password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }

        // Delete Account Section
        [DataType(DataType.Password)]
        [Display(Name = "Enter your password to confirm deletion")]
        public string? DeletePassword { get; set; }

        // User Info (for display)
        public string? UserType { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Phone { get; set; }
        public DateTime? JoinedDate { get; set; }
    }
}