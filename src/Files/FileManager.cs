using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jpp.Files
{
    public class FileManager
    {
        private string _basePath;
        private readonly ILogger _logger;

        public FileManager(IConfiguration configuration, ILogger<FileManager> logger)
        {
            _logger = logger;
            _basePath = configuration["BASEPATH"];
        }

        public async Task<Guid> CreateFile(byte[] data, string fileName)
        {
            Guid directoryId = Guid.NewGuid();
            
            var uploads = Path.Combine(_basePath, directoryId.ToString());
            var fullFileName = Path.Combine(uploads, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            using (var stream = new FileStream(fullFileName, FileMode.Create))
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            return directoryId;
        }

        public async Task<Guid> CreateFile(Stream data, string fileName)
        {
            Guid directoryId = Guid.NewGuid();

            var uploads = Path.Combine(_basePath, directoryId.ToString());
            var fullFileName = Path.Combine(uploads, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullFileName));

            using (var stream = new FileStream(fullFileName, FileMode.Create))
            {
                await data.CopyToAsync(stream);
            }

            return directoryId;
        }

        public async Task<(Stream, string)> GetFile(Guid fileId)
        {
            var contents = Directory.GetFiles(_basePath);
            
            var memory = new MemoryStream();
            var path = contents.First();
            return (File.Open(path, FileMode.Open), Path.GetFileName(path));
        }
    }
}
