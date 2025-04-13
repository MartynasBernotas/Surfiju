using Microsoft.AspNetCore.Http;

namespace DevKnowledgeBase.Application.Services
{
    public class FileService : IFileService
    {

        public FileService()
        {
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative URL of the uploaded file
            return $"{fileName}";
        }
    }
}
