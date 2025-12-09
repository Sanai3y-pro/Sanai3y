using sanaiy.BLL.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sanaiy.BLL.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResultDto> UploadImageAsync(byte[] fileBytes, string fileName, string folder);
        Task<FileUploadResultDto> UploadDocumentAsync(byte[] fileBytes, string fileName, string folder);
        Task<bool> DeleteFileAsync(string filePath);
        string GetFileUrl(string filePath);
    }
}