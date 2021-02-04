using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Sample.Azure.Translator.App.Models;
using Sample.Azure.Translator.App.Services;

namespace Sample.Azure.Translator.App
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await RunProcess(host, args).ConfigureAwait(true);

            //return host.RunAsync();
        }

        private static async Task RunProcess(IHost host, string[] args)
        {
            var text = string.Empty;

            // code here
            if(args.Length > 0)
            {
                var localFileService = host.Services.GetRequiredService<ILocalFileService>();

                if (localFileService.Exists(args[0]))
                {
                    text = await localFileService.ReadAsync(args[0]);
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                Console.WriteLine("Please type to want translation then press a return key (or enter key).");
                Console.WriteLine("If want to exit this app, press 'ctrl' + 'c'.");
                do
                {
                    Console.WriteLine("Input text:");
                    text = Console.ReadLine();
                }

                while (string.IsNullOrWhiteSpace(text));
            }

            var translatorService = host.Services.GetRequiredService<ITranslatorService>();

            var source = new TranslationRequestModel
            {
                Inputs = new TranslationRequestInputModel[]
                {
                    new TranslationRequestInputModel(text),
                },
                TranslateToLanguages = new string[] { "en", "ru", "ja" },
            };

            try
            {
                var result = await translatorService.TranslateAsync(source);

                Console.WriteLine($"Result: {result.ToJson()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
         
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // DI here
            services.AddTransient<ILocalFileService, LocalFileService>();
            services.AddTransient<ITranslatorService, TranslatorService>();

            return services;
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, configuration) =>
            {
                var currentDirectory = Directory.GetCurrentDirectory();
                
                configuration.SetBasePath(currentDirectory);

                configuration.SetFileLoadExceptionHandler(ctx =>
                {
                    if (ctx.Exception != null)
                    {
                        Console.WriteLine($"File load exception: {ctx.Exception.Message}");
                    }
                });

                configuration.Sources.Clear();

                IHostEnvironment env = hostingContext.HostingEnvironment;

                configuration
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);

            })
            .ConfigureServices((_, services) =>
            {
                var configuration = _.Configuration;

                services.AddOptions<AzureTranslatorConnectionOptions>().Configure(options =>
                {
                    configuration.GetSection(AzureTranslatorConnectionOptions.Name).Bind(options);
                });

                ConfigureServices(services);
            }).ConfigureLogging(builder =>
            {
                builder.AddConsole();
            });

    }

    public static class ObjectExtensions
    {
        public static string ToJson<T>(this T obj, JsonSerializerOptions options = null)
        {
            var actualOptions = options ?? new JsonSerializerOptions { 
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.Cyrillic),
            };

            return JsonSerializer.Serialize<T>(obj, actualOptions);
        }
    }
}
