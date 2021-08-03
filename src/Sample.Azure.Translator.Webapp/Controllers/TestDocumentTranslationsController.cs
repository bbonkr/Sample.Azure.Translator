
using kr.bbon.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using kr.bbon.Azure.Translator.Services;
using kr.bbon.Azure.Translator.Services.Strategies;
using kr.bbon.AspNetCore;
using kr.bbon.AspNetCore.Filters;
using Microsoft.Extensions.Options;
using System;
using Azure.AI.Translation.Document;
using Azure;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sample.Azure.Translator.Webapp.Models.DocumentTranslations;
using System.Linq;
using kr.bbon.Azure.Translator.Services.Models.DocumentTranslation.TranslationRequest;
using System.Net;
using kr.bbon.AspNetCore.Models;
using System.Text.Json;
using Azure.Storage.Sas;

namespace Sample.Azure.Translator.Webapp.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Area(DefaultValues.AreaName)]
    [Route(DefaultValues.RouteTemplate)]
    [ApiExceptionHandlerFilter]
    public class TestDocumentTranslationsController : ApiControllerBase
    {
        public TestDocumentTranslationsController(
            IStorageService storageService,
            IDocumentTranslationService documentTranslationService,
            IOptionsMonitor<AzureTranslatorOptions> azureTranslatorConnectionOptionsAccessor,
            ITranslatedDocumentNamingStrategy documentNamingStrategy,
            ILoggerFactory loggerFactory)
        {
            this.storageService = storageService;
            this.documentNamingStrategy = documentNamingStrategy;
            azureTranslatorOptions = azureTranslatorConnectionOptionsAccessor.CurrentValue;
            logger = loggerFactory.CreateLogger<TestDocumentTranslationsController>();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponseModel<DocumentTranslationResponseModel>))]
        public async Task<IActionResult> Translate(TranslateRequestModel model)
        {
            var documentTranslationClient = GetDocumentTranslationClient();

            var sourceUri = storageService.GenerateBlobSasUri(model.Name, BlobSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1));
            //var sourceUri = storageService.GenerateContainerSasUri(BlobContainerSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1));
            var translationSource = new TranslationSource(new Uri(sourceUri));
            translationSource.LanguageCode = model.CriteriaLanguage;

            //var nameTokens = model.Name.Split(".");
            //var namePart = String.Join(".", nameTokens.Take(nameTokens.Length - 1));

            //translationSource.Filter = new DocumentFilter
            //{
            //    Prefix = namePart,

            //};

            //foreach (var language in model.TargetLanguages)
            //{
            //    var result = await storageService.CreateAsync(documentNamingStrategy.GetTranslatedDocumentName(model.Name, language), string.Empty, "text/html");
            //}

            var translationTargets = model.TargetLanguages
               .Select(x => new TranslationTarget(new Uri(storageService.GenerateBlobSasUri(documentNamingStrategy.GetTranslatedDocumentName(model.Name, x), BlobSasPermissions.All, DateTimeOffset.UtcNow.AddMonths(1))), x))
               .ToList();

            //var translationTargets = model.TargetLanguages
            //   .Select(x => new TranslationTarget(new Uri(storageService.GenerateContainerSasUri(BlobContainerSasPermissions.All, DateTimeOffset.UtcNow.AddDays(1))), x))
            //   .ToList();

            //foreach(var target in translationTargets)
            //{
            //}

            var documentTranslationInput = new DocumentTranslationInput(translationSource, translationTargets);
            //documentTranslationInput.StorageType = StorageInputType.File;

            var operation = await documentTranslationClient.StartTranslationAsync(documentTranslationInput);

            var documentStatus = await operation.WaitForCompletionAsync();
            await foreach(var document in documentStatus.Value)
            {
                
            }


            return StatusCode(System.Net.HttpStatusCode.Accepted, new DocumentTranslationResponseModel
            {
                Id = operation.Id,
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponseModel<TranslationStatus>))]
        public async Task<IActionResult> GetTranslationStatus(string id)
        {
            TranslationStatus translationStatus = null;
            var documentTranslationClient = GetDocumentTranslationClient();
            await foreach (var status in documentTranslationClient.GetAllTranslationStatusesAsync().AsPages())
            {
                translationStatus = status.Values.Where(x => x.Id == id).FirstOrDefault();
                
                var json = JsonSerializer.Serialize(status.Values);

                if (translationStatus != null)
                {
                    break;
                }
            }

            if (translationStatus == null)
            {
                throw new ApiHttpStatusException<object>(HttpStatusCode.NotFound, "Operation not found", null);
            }

            return StatusCode(HttpStatusCode.OK, translationStatus);
        }

        private DocumentTranslationClient GetDocumentTranslationClient()
        {
            var keyCredential = new AzureKeyCredential(azureTranslatorOptions.SubscriptionKey);
            var documentTranslationClient = new DocumentTranslationClient(GetEndpoint(), keyCredential);

            return documentTranslationClient;
        }

        private Uri GetEndpoint()
        {
            return new Uri($"https://{azureTranslatorOptions.ResourceName}.cognitiveservices.azure.com");
        }

        private readonly IStorageService storageService;
        private readonly IDocumentTranslationService documentTranslationService;
        private readonly ITranslatedDocumentNamingStrategy documentNamingStrategy;
        private readonly AzureTranslatorOptions azureTranslatorOptions;
        private readonly ILogger logger;
    }
}
