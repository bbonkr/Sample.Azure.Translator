using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using kr.bbon.Azure.Translator.Services;
using kr.bbon.Azure.Translator.Services.Strategies;

using Swashbuckle.AspNetCore.SwaggerGen;
using kr.bbon.AspNetCore;

namespace Sample.Azure.Translator.Webapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppOptions>(Configuration.GetSection(AppOptions.Name));
            services.Configure<AzureTranslatorConnectionOptions>(Configuration.GetSection(AzureTranslatorConnectionOptions.Name));
            services.Configure<AzureStorageOptions>(Configuration.GetSection(AzureStorageOptions.Name));            

            services.AddTransient<IStorageService<TranslateAzureBlobStorageContainer>, AzureBlobStorageService<TranslateAzureBlobStorageContainer>>();
            services.AddTransient<ITextTranslatorService, TextTranslatorService>();
            services.AddTransient<IDocumentTranslationService, DocumentTranslationService>();

            services.AddTransient<ITranslatedDocumentNamingStrategy, TranslatedDocumentNamingStrategy>();

            var defaultVersion = new ApiVersion(1, 0);     

            services.AddControllers();

            services.AddApiVersioningAndSwaggerGen(defaultVersion);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUIWithApiVersioning();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
