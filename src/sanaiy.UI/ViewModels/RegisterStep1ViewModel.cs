using System.ComponentModel.DataAnnotations;

namespace sanaiy.UI.ViewModels
{
    public class RegisterStep1ViewModel
    {
        [Required]
        [StringLength(100)]
        public string FName { get; set; }

        [Required]
        [StringLength(100)]
        public string LName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        // Address
        [Required]
        public string FullAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string AddressType { get; set; }
        public string? OtherAddressType { get; set; }

        public bool IsDefaultAddress { get; set; }

        // Who are you registering as?
        [Required]
        public string RegisterAs { get; set; }   // "client" or "craftsman"

        public IFormFile? ProfileImage { get; set; }
        public string? ProfileImagePath { get; set; }
    }
}
