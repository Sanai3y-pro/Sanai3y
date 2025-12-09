using Microsoft.AspNetCore.Hosting;
using sanaiy.BLL.DTOs.Common;
using sanaiy.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace sanaiy.BLL.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private readonly string[] _allowedDocExtensions = { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public FileService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        public async Task<FileUploadResultDto> UploadImageAsync(byte[] fileBytes, string fileName, string folder)
        {
            try
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                if (!Array.Exists(_allowedImageExtensions, ext => ext == extension))
                    return new FileUploadResultDto { Success = false, Message = "نوع الملف غير مسموح" };

                if (fileBytes.Length > _maxFileSize)
                    return new FileUploadResultDto { Success = false, Message = "حجم الملف كبير جداً (الحد الأقصى 5 ميجابايت)" };

                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads", folder);
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                var relativePath = Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");

                return new FileUploadResultDto
                {
                    Success = true,
                    Message = "تم رفع الملف بنجاح",
                    FilePath = relativePath,
                    FileName = uniqueFileName,
                    FileSize = fileBytes.Length
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResultDto { Success = false, Message = $"فشل رفع الملف: {ex.Message}" };
            }
        }

        public async Task<FileUploadResultDto> UploadDocumentAsync(byte[] fileBytes, string fileName, string folder)
        {
            try
            {
                var extension = Path.GetExtension(fileName).ToLowerInvariant();
                if (!Array.Exists(_allowedDocExtensions, ext => ext == extension))
                    return new FileUploadResultDto { Success = false, Message = "نوع الملف غير مسموح" };

                if (fileBytes.Length > _maxFileSize)
                    return new FileUploadResultDto { Success = false, Message = "حجم الملف كبير جداً (الحد الأقصى 5 ميجابايت)" };

                var uploadsFolder = Path.Combine(_webHost.WebRootPath, "uploads", folder);
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                var relativePath = Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");

                return new FileUploadResultDto
                {
                    Success = true,
                    Message = "تم رفع الملف بنجاح",
                    FilePath = relativePath,
                    FileName = uniqueFileName,
                    FileSize = fileBytes.Length
                };
            }
            catch (Exception ex)
            {
                return new FileUploadResultDto { Success = false, Message = $"فشل رفع الملف: {ex.Message}" };
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_webHost.WebRootPath, filePath);
                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileUrl(string filePath)
        {
            return $"/{filePath.Replace("\\", "/")}";
        }
    }
}
