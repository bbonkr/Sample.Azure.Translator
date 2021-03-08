using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Sample.Azure.Translator.Models;
using Sample.Azure.Translator.Services;
using Sample.Azure.Translator.Services.Models;
using Sample.Azure.Translator.Webapp.Models;

namespace Sample.Azure.Translator.Webapp.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Area("api")]
    [Route("[area]/v{version:apiVersion}/[controller]")]
    public class DocumentsController : ApiControllerBase
    {
        public DocumentsController(
            IAzureStorageBlobService<TranslateAzureStorageContainer> storageService,
            ILoggerFactory loggerFactory)
        {
            this.storageService = storageService;
            logger = loggerFactory.CreateLogger<DocumentsController>();
        }


        //[HttpPost]
        //[Route("file")]
        //public async Task<IActionResult> CreateDocumentFromFiles(List<IFormFile> files)
        //{
        //    try
        //    {
        //        var resultSet = new List<BlobCreateResultModel>();

        //        if (files.Count > 0)
        //        {
        //            foreach (var file in files)
        //            {
        //                using (var stream = file.OpenReadStream())
        //                {
        //                    var result = await storageService.CreateAsync(file.Name, stream, file.ContentType);
        //                    resultSet.Add(result);
        //                }
        //            }

        //            return StatusCode(HttpStatusCode.Created, resultSet);
        //        }

        //        return StatusCode(HttpStatusCode.BadRequest, "Does not be provided files.");

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, ex.Message);

        //        return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
        //    }
        //}

        [HttpPost]
        //[Route("string")]
        public async Task<IActionResult> CreateDocument([FromForm] Documents.CreateRequestModel model)
        {
            var message = "";
            try
            {
                var resultSet = new List<BlobCreateResultModel>();

                if (model.Files != null && model.Files.Count() > 0)
                {
                    foreach (var file in model.Files)
                    {
                        using (var stream = file.OpenReadStream())
                        {
                            var result = await storageService.CreateAsync(file.FileName, stream, file.ContentType);
                            resultSet.Add(result);
                        }
                    }

                    return StatusCode(HttpStatusCode.Created, resultSet);
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        message = "Invalid request body.";
                        throw new InvalidRequestException<ErrorModel>(message, new ErrorModel
                        {
                            Code = 400,
                            Message = message,
                        });
                    }

                    var result = await storageService.CreateAsync(model.Name, model.Contents, model.ContentType);
                    resultSet.Add(result);
                }

                return StatusCode(HttpStatusCode.Created, resultSet);
            }
            catch (InvalidRequestException ex)
            {
                return StatusCode(HttpStatusCode.BadRequest, ex.Message, ex.GetDetails());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private readonly IAzureStorageBlobService<TranslateAzureStorageContainer> storageService;
        private readonly ILogger logger;
    }
}
