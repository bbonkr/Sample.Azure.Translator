﻿using System;
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

namespace Sample.Azure.Translator.Webapp.Controllers
{
    /// <summary>
    /// Document translation 
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    [Area("api")]
    [Route("[area]/v{version:apiVersion}/[controller]")]
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

                var documentTranslationRequestModel = new RequestModel
                {
                    Inputs = new BatchInput[]
                    {
                        new BatchInput
                        {
                             Source = new SourceInput
                             {
                                SourceUrl = storageService.GenerateSasUri(model.Name),
                                Language = model.CriteriaLanguage,
                             },
                             StorageType =  StorageInputTypes.File,
                             Targets = model.TargetLanguages.Select(language => new TargetInput
                             {
                                 Language =language,
                                 TargetUrl =storageService.GenerateSasUri(documentNamingStrategy.GetTranslatedDocumentName(result.Uri,language)),
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
        public async Task<IActionResult> GetTranslationJobStatusAsync(string id)
        {
            try
            {
                await documentTranslationService.GetJobStatusAsync(id);

                return StatusCode(HttpStatusCode.OK, id);
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