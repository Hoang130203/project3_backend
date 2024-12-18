using FileService.Models;
using FileService.Service;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }
        [RequestFormLimits(MultipartBodyLengthLimit = 152428800)]
        [RequestSizeLimit(152428800)]
        [HttpPost("upload")]
        public async Task<ActionResult<FileUploadResult>> UploadFile(IFormFile file)
        {
            var result = await _fileService.UploadAsync(file);
            if (!result.Success)
                return BadRequest(result);

            return Ok(result.Url);
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                var (fileStream, contentType, originalFileName) = await _fileService.GetFileAsync(fileName);
                return File(fileStream, contentType, originalFileName);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
