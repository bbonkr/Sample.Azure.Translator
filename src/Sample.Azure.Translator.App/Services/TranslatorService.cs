using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Sample.Azure.Translator.App.Models;

namespace Sample.Azure.Translator.App.Services
{
    public interface ITranslatorService
    {
        Task<ResponseModel<TranslationResultModel[]>> TranslateAsync(TranslationRequestModel model);
    }

    public class TranslatorService : ITranslatorService
    {
        public const string TRANSLATOR_ROUTE = "/translate?api-version=3.0";
        public const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
        public const string OCP_APIM_SUBSCRIPTION_REGION = "Ocp-Apim-Subscription-Region";
        public const string CONTENT_TYPE_KEY = "Content-Type";
        public const string CONTENT_TYPE_VALUE = "application/json";


        public TranslatorService(IOptions<AzureTranslatorConnectionOptions> azureTranslatorConnectionOptionsAccessor, ILoggerFactory loggerFactory) 
        {
            azureTranslatorConnectionOptions = azureTranslatorConnectionOptionsAccessor.Value;
            logger = loggerFactory.CreateLogger<TranslatorService>();
        }

        public async Task<ResponseModel<TranslationResultModel[]>> TranslateAsync(TranslationRequestModel model)
        {
            ValidateAzureTranslateConnectionOptions();
            ValidateRequestbody(model);

            var requestBody = model.Inputs.ToJson();

            using (var client = new HttpClient())
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = getRequestUri(model);

                    request.Headers.Add(OCP_APIM_SUBSCRIPTION_KEY, azureTranslatorConnectionOptions.SubscriptionKey);
                    request.Headers.Add(OCP_APIM_SUBSCRIPTION_REGION, azureTranslatorConnectionOptions.Region);
                    
                    request.Content = new StringContent(requestBody, Encoding.UTF8, CONTENT_TYPE_VALUE);
                    request.Content.Headers.ContentLength = Encoding.UTF8.GetBytes(requestBody).Length;
                    

                    var response = await client.SendAsync(request);
                    
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonSerializerOptions = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    };

                    if (response.IsSuccessStatusCode)
                    {
                        var resultModel = JsonSerializer.Deserialize<TranslationResultModel[]>(result, jsonSerializerOptions);


                        return ResponseModelFactory.Create(HttpStatusCode.OK, string.Empty, resultModel);
                    }
                    else
                    {
                        var resultModel = JsonSerializer.Deserialize<ErrorResponseModel>(result, jsonSerializerOptions);

                        return ResponseModelFactory.Create<TranslationResultModel[]>(response.StatusCode, resultModel.Error?.Message, null);
                    }
                }
            }
        }

        private Uri getRequestUri(TranslationRequestModel model)
        {
            var endpoint = azureTranslatorConnectionOptions.Endpoint;
            if (endpoint.EndsWith("/"))
            {
                endpoint = endpoint.Substring(0, endpoint.Length - 1);
            }

            var url = $"{endpoint}{TRANSLATOR_ROUTE}&to={String.Join("&to=", model.ToLanguages)}";

            if (!String.IsNullOrWhiteSpace(model.FromLanguage))
            {
                url = $"{url}&from={model.FromLanguage}";
            }

            if (!String.IsNullOrWhiteSpace(model.TextType) && !model.TextType.Equals(TextTypes.Plain, StringComparison.OrdinalIgnoreCase))
            {
                url = $"{url}&textType={model.TextType}";
            }

            if (!String.IsNullOrWhiteSpace(model.Category) && !model.Category.Equals(Categories.General, StringComparison.OrdinalIgnoreCase))
            {
                url = $"{url}&category={model.Category}";
            }

            if (!String.IsNullOrWhiteSpace(model.ProfanityAction) && !model.ProfanityAction.Equals(ProfanityActions.NoAction, StringComparison.OrdinalIgnoreCase))
            {
                url = $"{url}&profanityAction={model.ProfanityAction}";

                if (!String.IsNullOrWhiteSpace(model.ProfanityMarker) && !model.ProfanityMarker.Equals(ProfanityMarkers.Asterisk, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{url}&profanityMarker={model.ProfanityMarker}";
                }
            }

            var requestUri = new Uri(url);

            //logger.LogInformation($"Request uri: {requestUri.ToString()}");

            return requestUri;
        }

        private void ValidateAzureTranslateConnectionOptions()
        {
            var errorMessage = new List<string>();

            if (string.IsNullOrWhiteSpace(azureTranslatorConnectionOptions.Endpoint))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.Endpoint)} is required");
            }

            if (string.IsNullOrWhiteSpace(azureTranslatorConnectionOptions.Region))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.Region)} is required");
            }

            if (string.IsNullOrWhiteSpace(azureTranslatorConnectionOptions.SubscriptionKey))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.SubscriptionKey)} is required");
            }

            if (errorMessage.Count > 0)
            {
                throw new OptionsValidationException(AzureTranslatorConnectionOptions.Name, typeof(AzureTranslatorConnectionOptions), errorMessage.ToArray());
            }
        }

        private void ValidateRequestbody(TranslationRequestModel model)
        {
            var errorMessage = new List<string>();

            var inputsCount = model.Inputs.Count();

            if (inputsCount == 0)
            {
                errorMessage.Add("Text to translate is required.");
            }

            if (inputsCount > 100)
            {
                errorMessage.Add("The array can have at most 100 elements.");
            }

            foreach (var input in model.Inputs)
            {
                var contentLength = input.Text.Length; // Encoding.UTF8.GetBytes(input.Text).Length;
                if (contentLength > 10000)
                {
                    errorMessage.Add("The entire text included in the request cannot exceed 10,000 characters including spaces.");
                    break;
                }
            }

            if (errorMessage.Count > 0)
            {
                throw new InvalidRequestException(string.Join($"{Environment.NewLine}- ", errorMessage.ToArray()));
            }
        }


        private readonly AzureTranslatorConnectionOptions azureTranslatorConnectionOptions;
        private readonly ILogger logger;
    }

    
}
