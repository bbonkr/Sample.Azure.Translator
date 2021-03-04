using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Sample.Azure.Translator.App.Services
{
    public interface ILocalFileService
    {
        Task<string> ReadAsync(string filePath);
        
        bool Exists(string filePath);
    }
    public class LocalFileService : ILocalFileService
    {
        public LocalFileService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<LocalFileService>();
        }

        public Task<string> ReadAsync(string filePath)
        {
            if (!Exists(filePath))
            {
                throw new FileNotFoundException($"File does not find. ({filePath})");
            }

            return File.ReadAllTextAsync(filePath);
        }

        public bool Exists(string filePath) => File.Exists(filePath);
        //{
        //    var path = Path.GetFullPath(filePath, Directory.GetCurrentDirectory());

        //    return File.Exists(filePath);
        //}

        private ILogger logger;
    }
}
