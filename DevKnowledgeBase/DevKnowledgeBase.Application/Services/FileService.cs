using Microsoft.AspNetCore.Http;

namespace DevKnowledgeBase.Application.Services
{
    public class FileService : IFileService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public FileService()
        {
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException($"File type '{extension}' is not allowed. Permitted types: {string.Join(", ", AllowedExtensions)}");

            if (file.Length > MaxFileSizeBytes)
                throw new InvalidOperationException($"File size exceeds the maximum allowed size of 5 MB.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Use a GUID name to avoid path traversal via the original filename
            var safeFileName = Guid.NewGuid().ToString() + extension.ToLowerInvariant();
            var filePath = Path.Combine(uploadsFolder, safeFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return safeFileName;
        }
    }
}
