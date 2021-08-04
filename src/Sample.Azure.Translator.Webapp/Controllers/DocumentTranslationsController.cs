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
using kr.bbon.Azure.Translator.Services.Models.DocumentTranslation.GetJobStatus;
using Azure.Storage.Sas;

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
            IStorageService storageService,
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
        [Produces(typeof(DocumentTranslationResponseModel))]
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

                //var result = await storageService.FindByNameAsync(model.Name);


                //var containerSasUri = storageService.GenerateContainerSasUri();

                var nameTokens = model.Name.Split('.');
                var namePart = string.Join(".", nameTokens.Take(nameTokens.Length - 1));

                var containerSasUri = "https://chtranslatorstroage.blob.core.windows.net/document-translation?sp=rwl&st=2021-08-03T07:46:05Z&se=2021-08-03T15:46:05Z&spr=https&sv=2020-08-04&sr=c&sig=D8uNMCPgOpEN9AGG6d%2BCS1TOAnEhXON1NNliUtpM174%3D";

                var documentTranslationRequestModel = new DocumentTranslationRequestModel
                {
                    Inputs = new BatchInput[]
                    {
                        new BatchInput
                        {
                             Source = new SourceInput
                             {
                                //SourceUrl = storageService.GenerateBlobSasUri(model.Name, BlobSasPermissions.List | BlobSasPermissions.Read, DateTimeOffset.UtcNow + TimeSpan.FromDays(3)),
                                //SourceUrl = storageService.GenerateContainerSasUri(BlobContainerSasPermissions.All, DateTimeOffset.UtcNow + TimeSpan.FromDays(3)),
                                SourceUrl = containerSasUri ,
                                StorageSource = StorageSources.AzureBlob,

                                Filter= new Filter
                                {
                                    //Prefix = namePart,
                                    //Suffix = ".html",
                                    Suffix=model.Name,
                                },
                                Language = model.CriteriaLanguage,
                             },
                             //StorageType =  StorageInputTypes.File,
                             StorageType =  StorageInputTypes.Folder,
                             Targets = model.TargetLanguages.Select(language => new TargetInput
                             {
                                 //TargetUrl = storageService.GenerateBlobSasUri(documentNamingStrategy.GetTranslatedDocumentName(model.Name,language), BlobSasPermissions.List | BlobSasPermissions.Write | BlobSasPermissions.Create , DateTimeOffset.UtcNow + TimeSpan.FromDays(3)),
                                 //TargetUrl = storageService.GenerateContainerSasUri(BlobContainerSasPermissions.All, DateTimeOffset.UtcNow + TimeSpan.FromDays(3)),
                                 //TargetUrl="https://chtranslatorstroage.blob.core.windows.net/document-translation?sp=rw&st=2021-08-03T07:46:05Z&se=2021-08-03T15:46:05Z&spr=https&sv=2020-08-04&sr=c&sig=7%2FxohLKKDT2qywxgiFgpqVGdMXOm7lo5opITh1V2Vr4%3D",
                                 TargetUrl = containerSasUri,
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
        [Produces(typeof(DocumentTranslationJobStatusResponseModel))]
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

        private readonly IStorageService storageService;
        private readonly IDocumentTranslationService documentTranslationService;
        private readonly ITranslatedDocumentNamingStrategy documentNamingStrategy;
        private readonly ILogger logger;
    }
}
