using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Craftsman
{
    public class CraftsmanApplicationDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? NationalID { get; set; }
        public string? Category { get; set; }
        public int? YearsOfExperience { get; set; }
        public string Status { get; set; } = string.Empty;

        // Verification Documents (URLs)
        public string? IDCardImageUrl { get; set; }
        public string? DrugTestFileUrl { get; set; }
        public string? CriminalRecordFileUrl { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}