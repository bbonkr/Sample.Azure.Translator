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
using Sample.Azure.Translator.Services.Strategies;
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
            IStorageService<TranslateAzureBlobStorageContainer> storageService,
            IDocumentTranslationService documentTranslationService,
            ITranslatedDocumentNamingStrategy documentNamingStrategy,
            ILoggerFactory loggerFactory)
        {
            this.storageService = storageService;
            this.documentNamingStrategy = documentNamingStrategy;
            this.documentTranslationService = documentTranslationService;
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
            catch (HttpStatusException ex)
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
        public async Task<IActionResult> CreateDocumentAsync([FromForm] Documents.CreateRequestModel model)
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

                        throw new HttpStatusException<ErrorModel>(HttpStatusCode.BadRequest, message, new ErrorModel
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
            catch (HttpStatusException ex)
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
            catch (HttpStatusException ex)
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
        [Route("translate")]
        public async Task<IActionResult> TranslateAsync(Documents.TranslateRequestModel model)
        {
            var message = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    message = "Request body is invalid.";
                    throw new HttpStatusException<ErrorModel>(HttpStatusCode.BadRequest, message, new ErrorModel
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = message,
                    });
                }

                var result = await storageService.FindByNameAsync(model.Name);

                var documentTranslationRequestModel = new DocumentTranslationRequestModel
                {
                    Inputs = new DocumentTranslationBatchRequest[]
                    {
                        new DocumentTranslationBatchRequest
                        {
                             Source = new DocumentTranslationSourceInput
                             {
                                SourceUrl = storageService.GenerateSasUri(model.Name), //new Uri(result.Uri).AbsoluteUri,
                                Language = model.CriteriaLanguage,
                             },
                             StorageType =  DocumentTranslationStorageInputTypes.File,
                             Targets = model.TargetLanguages.Select(language =>   new DocumentTranslationTargetInput
                                {
                                    Language =language,
                                    TargetUrl =storageService.GenerateSasUri( documentNamingStrategy.GetTranslatedDocumentName(result.Uri,language )),
                                }),
                        },
                    },
                };

                var translationRequestResult = await documentTranslationService.RequestTranslation(documentTranslationRequestModel);

                return StatusCode(HttpStatusCode.Accepted, translationRequestResult);
            }
            catch (HttpStatusException ex)
            {
                return StatusCode(ex.StatusCode, ex.Message, ex.GetDetails());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                return StatusCode(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private readonly IStorageService<TranslateAzureBlobStorageContainer> storageService;
        private readonly IDocumentTranslationService documentTranslationService;
        private readonly ITranslatedDocumentNamingStrategy documentNamingStrategy;
        private readonly ILogger logger;
    }
}
