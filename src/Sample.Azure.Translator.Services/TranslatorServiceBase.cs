using System;
using System.Collections.Generic;

using Microsoft.Extensions.Options;

namespace Sample.Azure.Translator.Services
{
    public abstract class TranslatorServiceBase: ServiceBase
    {
        protected const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
        protected const string OCP_APIM_SUBSCRIPTION_REGION = "Ocp-Apim-Subscription-Region";
        protected const string CONTENT_TYPE_KEY = "Content-Type";
        protected const string CONTENT_TYPE_VALUE = "application/json";

        public TranslatorServiceBase(IOptionsMonitor<AzureTranslatorConnectionOptions> azureTranslatorConnectionOptionsAccessor)
        {
            options = azureTranslatorConnectionOptionsAccessor.CurrentValue;
        }

        /// <summary>
        /// Api route
        /// </summary>
        protected abstract string Route { get; }      

        /// <summary>
        /// Api base url
        /// </summary>
        /// <returns></returns>
        protected virtual string GetApiBaseUrl()
        {
            var endpoint = options.Endpoint;
            if (endpoint.EndsWith("/"))
            {
                endpoint = endpoint.Substring(0, endpoint.Length - 1);
            }

            var url = $"{endpoint}{Route}";

            return url;
        }

        protected virtual void ValidateAzureTranslateConnectionOptions()
        {
            var errorMessage = new List<string>();

            if (string.IsNullOrWhiteSpace(options.Endpoint))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.Endpoint)} is required");
            }

            if (string.IsNullOrWhiteSpace(options.Region))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.Region)} is required");
            }

            if (string.IsNullOrWhiteSpace(options.SubscriptionKey))
            {
                errorMessage.Add($"{nameof(AzureTranslatorConnectionOptions.SubscriptionKey)} is required");
            }

            if (errorMessage.Count > 0)
            {
                throw new OptionsValidationException(AzureTranslatorConnectionOptions.Name, typeof(AzureTranslatorConnectionOptions), errorMessage.ToArray());
            }
        }

        protected readonly AzureTranslatorConnectionOptions options;
    }
}
