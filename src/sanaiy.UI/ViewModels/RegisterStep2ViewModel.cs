namespace sanaiy.UI.ViewModels
{
    public class RegisterStep2ViewModel
    {

        public string NationalID { get; set; }

        public int? YearsOfExperience { get; set; }

        public string Category { get; set; } // Craft type

        public string? Bio { get; set; }

        // Files
        public IFormFile? IDCardImage { get; set; }
        public IFormFile? DrugTestFile { get; set; }
        public IFormFile? CriminalRecordFile { get; set; }
    }
}

