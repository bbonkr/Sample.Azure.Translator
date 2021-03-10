using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;
using kr.bbon.AspNetCore.Options;

namespace Sample.Azure.Translator.Webapp
{
    public class ConfigureSwaggerOptions : ConfigureSwaggerOptionsBase
    {
        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) : base(provider)
        {
        }

        public override string AppTitle { get; }
    }
}
