using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using Microsoft.Extensions.Logging;

namespace Jpp.Files.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly ILogger _logger;
        private FileManager _files;

        public FilesController(ILogger<FilesController> logger, FileManager files)
        {
            _logger = logger;
            _files = files;
        }

        [HttpPost("Base64")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(OperationId = "uploadFileBase64")]
        public async Task<IActionResult> UploadFileBase64Async([FromBody] Base64Model model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Base64)) return BadRequest();
                if (string.IsNullOrWhiteSpace(model.FileName)) return BadRequest();

                var extension = Path.GetExtension(model.FileName).ToLowerInvariant();
                if (GetMimeTypes().All(e => e.Key != extension)) return BadRequest();

                var bytes = Convert.FromBase64String(model.Base64);

                Guid fileId = await _files.CreateFile(bytes, model.FileName);

                return new OkObjectResult(fileId);
            }                                
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]        
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(OperationId = "uploadFile")]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file.Length <= 0) return BadRequest();

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (GetMimeTypes().All(e => e.Key != extension)) return BadRequest();

                using Stream dataStream = file.OpenReadStream();
                Guid fileId = await _files.CreateFile(dataStream, file.FileName);

                return new OkObjectResult(fileId);
            }                                
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id:Guid}")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(OperationId = "getFile")]
        public async Task<IActionResult> GetFileAsync(Guid id)
        {
            try
            {
                (Stream data, string filename) = await _files.GetFile(id);
                using (data)
                {
                    return File(data, GetContentType(filename), Path.GetFileName(filename));
                }
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }     
        }

        [HttpGet("Types")]
        [Produces("application/json")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Dictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(OperationId = "getFileTypes")]
        public IActionResult GetFileTypes()
        {
            try
            {
                return new OkObjectResult(GetMimeTypes());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private static string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private static Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                { ".bmp", "image/bmp" },
                { ".css", "text/css" },
                { ".csv", "text/csv" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".gif", "image/gif" },
                { ".htm", "text/html" },
                { ".html", "text/html" },
                { ".ico", "image/vnd.microsoft.icon" },
                { ".jpeg", "image/jpeg" },
                { ".jpg", "image/jpeg" },
                { ".js", "text/javascript" },
                { ".json", "application/json" },
                { ".odp", "application/vnd.oasis.opendocument.presentation" },
                { ".ods", "application/vnd.oasis.opendocument.spreadsheet" },
                { ".odt", "application/vnd.oasis.opendocument.text" },
                { ".png", "image/png" },
                { ".pdf", "application/pdf" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".rar", "application/x-rar-compressed" },
                { ".rtf", "application/rtf" },
                { ".svg", "image/svg+xml" },
                { ".txt", "text/plain" },
                { ".vsd", "application/vnd.visio" },
                { ".xhtml", "application/xhtml+xml" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".xml", "application/xml" },
                { ".zip", "application/zip" },
                { ".7z", "application/x-7z-compressed" }
            };
        }
    }

    public class Base64Model
    {
        public string FileName { get; set; }
        public string Base64 { get; set; }
    }
}
