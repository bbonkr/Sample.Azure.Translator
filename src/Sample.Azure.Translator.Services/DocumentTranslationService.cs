﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Sample.Azure.Translator.Services.Models;

namespace Sample.Azure.Translator.Services
{
    public interface IDocumentTranslationService
    {
        Task<DocumentTranslationRequestResponseModel> RequestTranslation(DocumentTranslationRequestModel model, CancellationToken cancellationToken = default);
    }

    public class DocumentTranslationService : TranslatorServiceBase, IDocumentTranslationService
    {
        public DocumentTranslationService(
            IOptionsMonitor<AzureTranslatorConnectionOptions> azureTranslatorConnectionOptionsAccessor,
            ILoggerFactory loggerFactory)
            : base(azureTranslatorConnectionOptionsAccessor)
        {
            logger = loggerFactory.CreateLogger<DocumentTranslationService>();
        }

        public async Task<DocumentTranslationRequestResponseModel> RequestTranslation(DocumentTranslationRequestModel model, CancellationToken cancellationToken = default)
        {
            ValidateAzureTranslateConnectionOptions();
            ValidateRequestbody(model);

            DocumentTranslationRequestResponseModel result = null;

            var requestBody = model.ToJson();

            using (var client = new HttpClient())
            {

                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri(GetApiBaseUrl());

                    request.Headers.Add(OCP_APIM_SUBSCRIPTION_KEY, options.SubscriptionKey);
                    request.Headers.Add(OCP_APIM_SUBSCRIPTION_REGION, options.Region);

                    request.Content = new StringContent(requestBody, Encoding.UTF8, CONTENT_TYPE_VALUE);

                    var response = await client.SendAsync(request, cancellationToken);

                    var resultJson = await response.Content?.ReadAsStringAsync();

                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };

                    if (response.IsSuccessStatusCode)
                    {
                        logger.LogInformation($"${Tag} The request has been processed. => Translated.");

                        if (response.Headers.Contains("Operation-Location"))
                        {
                            result = response.Headers.GetValues("Operation-Location").Select(operationLocation => new DocumentTranslationRequestResponseModel
                            {
                                Id = operationLocation,
                            }).FirstOrDefault();
                        }
                        else
                        {
                            var code = "No operation location";
                            var message = "Translation operation could not find.";
                            throw new SomethingWrongException<DocumentTranslationErrorResponseModel>(message, new DocumentTranslationErrorResponseModel
                            {
                                Error = new DocumentTranslationErrorModel
                                {
                                    Code = code,
                                    Message = message,
                                }
                            });
                        }
                    }
                    else
                    {
                        var errorResult = JsonSerializer.Deserialize<DocumentTranslationErrorResponseModel>(resultJson, jsonSerializerOptions);

                        logger.LogInformation($"${Tag} The request does not has been processed. => Not  Translated.");

                        throw new SomethingWrongException<DocumentTranslationErrorResponseModel>(errorResult.Error.Message, errorResult);
                    }
                }
            }

            return result;
        }

        private void ValidateRequestbody(DocumentTranslationRequestModel model)
        {
            var errors = new List<string>();

            if (model.Inputs == null || model.Inputs.Count() == 0)
            {
                errors.Add("Inputs is required.");
            }

            if (model.Inputs.Any(x => string.IsNullOrWhiteSpace(x.Source.SourceUrl)))
            {
                errors.Add("Source url is required.");
            }

            if (model.Inputs.Any(x => x.Targets.Any(y => string.IsNullOrWhiteSpace(y.TargetUrl))))
            {
                errors.Add("Target url is required.");
            }

            if (errors.Count > 0)
            {
                throw new HttpStatusException<IEnumerable<string>>(HttpStatusCode.BadRequest, errors.FirstOrDefault(), errors);
            }
        }

        protected override string Route { get => "/translator/text/batch/v1.0-preview.1/batches"; }
        protected override string Tag { get => "[Azure Translator: Document]"; }

        private readonly ILogger logger;
    }
}
