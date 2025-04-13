using DevKnowledgeBase.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevKnowledgeBase.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {

        private readonly IFileService _fileService;
        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Save the file (e.g., to local storage or cloud storage)
            var fileUrl = await _fileService.SaveFileAsync(file);

            return Ok(new { Url = fileUrl });
        }

        [HttpGet("photos/{fileName}")]
        [AllowAnonymous]
        public IActionResult GetPhoto(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, contentType);
        }
    }
}
