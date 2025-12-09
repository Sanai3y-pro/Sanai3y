using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.DTOs.Common
{
    public class FileUploadResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FilePath { get; set; } // The path stored in DB (e.g., /images/user.jpg)
        public string? FileName { get; set; } // Original name
        public long FileSize { get; set; }
    }
}