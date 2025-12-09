namespace sanaiy.UI.ViewModels
{
    public class RegisterStep1TempDto
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string FullAddress { get; set; }
        public string City { get; set; }
        public string AddressType { get; set; }
        public bool IsDefaultAddress { get; set; }

        public string RegisterAs { get; set; }

        // Only store the FILE PATH, not IFormFile
        public string? ProfileImagePath { get; set; }
    }
}
