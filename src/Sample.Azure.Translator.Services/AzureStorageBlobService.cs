using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Sample.Azure.Translator.Services.Models;

namespace Sample.Azure.Translator.Services
{
    public interface IAzureStorageBlobService<T>
    {
        Task<BlobCreateResultModel> CreateAsync(string name, Stream stream, string contentType, CancellationToken token = default);
        Task<BlobCreateResultModel> CreateAsync(string name, string contents, string contentType, CancellationToken token = default);
    }

   
    public class AzureStorageBlobService<T> : IAzureStorageBlobService<T> where T: AzureStorageContainerBase
    {
        public AzureStorageBlobService(IOptionsMonitor<AzureStorageOptions> azureStorageOptionsAccessor, ILoggerFactory loggerFactory)
        {
            this.options = azureStorageOptionsAccessor.CurrentValue;
            this.logger = loggerFactory.CreateLogger<AzureStorageBlobService<T>>();

            var container = Activator.CreateInstance<T>();
            this.client = new BlobContainerClient(options.ConnectionString, container.GetContainerName());
        }

        public async Task<BlobCreateResultModel> CreateAsync(string name, Stream stream, string contentType = "", CancellationToken cancellationToken = default)
        {
            try
            {
                EnsureContainerCreated();

                var blobClient = client.GetBlobClient(name);

                var uploadOptions = new BlobUploadOptions();

                if (!string.IsNullOrWhiteSpace(contentType))
                {
                    uploadOptions.HttpHeaders = new BlobHttpHeaders { ContentType = contentType };
                }                

                var result = await blobClient.UploadAsync(stream, uploadOptions, cancellationToken);

                return new BlobCreateResultModel
                {
                    ContainerName = client.Name,
                    BlobName = blobClient.Name,
                    Uri = blobClient.Uri.ToString(),
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);

                throw;
            }
        }

        public async Task<BlobCreateResultModel> CreateAsync(string name, string contents, string contentType, CancellationToken token = default)
        {
            BlobCreateResultModel result;

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    await writer.WriteAsync(contents);
                    await writer.FlushAsync();
                    writer.Close();
                }

                result = await CreateAsync(name, stream, contentType, token);

                stream.Close();
            }

            return result;
        }

        public async Task FindByName(string name)
        {
            EnsureContainerCreated();

            await foreach(var item in client.GetBlobsAsync(prefix: name))
            {
               
            }
        }

        private void EnsureContainerCreated()
        {
            client.CreateIfNotExists(PublicAccessType.BlobContainer);
        }

        private readonly AzureStorageOptions options;
        private readonly BlobContainerClient client;
        private readonly ILogger logger;
    }

    public class AzureStorageOptions
    {
        public static string Name = "AzureStorage";
        public string ConnectionString { get; set; }
    }

    public abstract class AzureStorageContainerBase
    {
        public abstract string GetContainerName();
    }

    public class TranslateAzureStorageContainer : AzureStorageContainerBase
    {
        public override string GetContainerName()
        {
            return "document-translation-sample";
        }
    }

}
