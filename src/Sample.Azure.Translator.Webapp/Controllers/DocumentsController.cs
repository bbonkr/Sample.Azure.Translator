using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using kr.bbon.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using kr.bbon.Azure.Translator.Services;
using kr.bbon.Azure.Translator.Services.Models;
using kr.bbon.Azure.Translator.Services.Models.AzureStorage.Blob;
using Sample.Azure.Translator.Webapp.Models.Documents;
using kr.bbon.AspNetCore.Filters;
using kr.bbon.AspNetCore;

namespace Sample.Azure.Translator.Webapp.Controllers
{
    /// <summary>
    /// Azure Storage Account - Blob Storage
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Area(DefaultValues.AreaName)]
    [Route(DefaultValues.RouteTemplate)]
    [ApiExceptionHandlerFilter]
    public class DocumentsController : ApiControllerBase
    {
        public DocumentsController(
            IStorageService storageService,
            ILoggerFactory loggerFactory)
        {
            this.storageService = storageService;
            logger = loggerFactory.CreateLogger<DocumentsController>();
        }


        [HttpGet]
        [Route("{name}")]
        public async Task<IActionResult> FindByIdAsync(string name)
        {
            try
            {
                var result = await storageService.FindByNameAsync(name);

                return StatusCode(HttpStatusCode.OK, result);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message, ex.GetDetails());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocumentAsync([FromForm] CreateRequestModel model)
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

                        throw new ApiHttpStatusException<ErrorModel<int>>(HttpStatusCode.BadRequest, message, new ErrorModel<int>
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
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message, ex.GetDetails());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpDelete]
        [Route("{name}")]
        public async Task<IActionResult> DeleteDocumentAsync(string name)
        {
            try
            {
                var result = await storageService.DeleteAsync(name);

                if (!result)
                {
                    return StatusCode((int)HttpStatusCode.NotAcceptable);
                }

                return StatusCode((int)HttpStatusCode.Accepted);
            }
            catch (ApiException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message, ex.GetDetails());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private readonly IStorageService storageService;
        private readonly ILogger logger;
    }
}