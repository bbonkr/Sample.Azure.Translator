using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;
using kr.bbon.AspNetCore.Options;
using kr.bbon.AspNetCore;

namespace Sample.Azure.Translator.Webapp
{
    public class ConfigureSwaggerOptions : ConfigureSwaggerOptionsBase
    {
        public ConfigureSwaggerOptions(IOptionsMonitor<AppOptions> appOptionsAccessor, IApiVersionDescriptionProvider provider)
            : base(provider)
        {
            this.options = appOptionsAccessor.CurrentValue;
        }

        public override string AppTitle { get => options.Title; }
        
        private readonly AppOptions options;
    }
}
