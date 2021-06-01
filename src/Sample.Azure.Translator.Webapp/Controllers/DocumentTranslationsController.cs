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
using kr.bbon.Azure.Translator.Services.Models.DocumentTranslation.TranslationRequest;
using kr.bbon.Azure.Translator.Services.Strategies;
using Sample.Azure.Translator.Webapp.Models.DocumentTranslations;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Filters;

namespace Sample.Azure.Translator.Webapp.Controllers
{
    /// <summary>
    /// Document translation 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Area(DefaultValues.AreaName)]
    [Route(DefaultValues.RouteTemplate)]
    [ApiExceptionHandlerFilter]
    public class DocumentTranslationsController: ApiControllerBase
    {
        public DocumentTranslationsController(
            IStorageService<TranslateAzureBlobStorageContainer> storageService,
            IDocumentTranslationService documentTranslationService,
            ITranslatedDocumentNamingStrategy documentNamingStrategy,
            ILoggerFactory loggerFactory)
        {
            this.storageService = storageService;
            this.documentNamingStrategy = documentNamingStrategy;
            this.documentTranslationService = documentTranslationService;
            logger = loggerFactory.CreateLogger<DocumentTranslationsController>();
        }

        [HttpPost]
        [Produces(typeof(kr.bbon.Azure.Translator.Services.Models.DocumentTranslation.TranslationRequest.ResponseModel))]
        public async Task<IActionResult> TranslateAsync(TranslateRequestModel model)
        {
            var message = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    message = "Request body is invalid.";
                    throw new ApiHttpStatusException<ErrorModel<int>>(HttpStatusCode.BadRequest, message, new ErrorModel<int>
                    {
                        Code = (int)HttpStatusCode.BadRequest,
                        Message = message,
                    });
                }

                var result = await storageService.FindByNameAsync(model.Name);


                var containerSasUri = storageService.GenerateContainerSasUri();

                var documentTranslationRequestModel = new RequestModel
                {
                    Inputs = new BatchInput[]
                    {
                        new BatchInput
                        {
                             Source = new SourceInput
                             {
                                SourceUrl = storageService.GenerateBlobSasUri(model.Name),
                                StorageSource = StorageSources.AzureBlob,
                                //Filter= new Filter
                                //{
                                //    Prefix = "sample 2020",
                                //    Suffix = ".html",
                                //},
                                Language = model.CriteriaLanguage,
                             },
                             StorageType =  StorageInputTypes.File,
                             Targets = model.TargetLanguages.Select(language => new TargetInput
                             {
                                 TargetUrl = storageService.GenerateBlobSasUri(documentNamingStrategy.GetTranslatedDocumentName(model.Name,language)),
                                 Language = language,
                                 StorageSource = StorageSources.AzureBlob,                                 
                             }),
                        },
                    },
                };

                var translationRequestResult = await documentTranslationService.RequestTranslation(documentTranslationRequestModel);

                return StatusCode(HttpStatusCode.Accepted, translationRequestResult);
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

        [HttpGet]
        [Route("{id}")]
        [Produces(typeof(kr.bbon.Azure.Translator.Services.Models.DocumentTranslation.GetJobStatus.JobStatusResponseModel))]
        public async Task<IActionResult> GetTranslationJobStatusAsync(string id)
        {
            try
            {
                var result = await documentTranslationService.GetJobStatusAsync(id);

                if(result == null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

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

        private readonly IStorageService<TranslateAzureBlobStorageContainer> storageService;
        private readonly IDocumentTranslationService documentTranslationService;
        private readonly ITranslatedDocumentNamingStrategy documentNamingStrategy;
        private readonly ILogger logger;
    }
}
