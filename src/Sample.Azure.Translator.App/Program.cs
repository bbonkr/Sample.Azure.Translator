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
            var html = false;
            var from = string.Empty;
            var isTranslationEachLanguage = false;
            var toLanguages = new[] { "en", "ru", "ja" };

            // code here
            foreach (var arg in args)
            {
                if (arg.Trim().StartsWith("--"))
                {
                    if (arg.Trim().Equals("--html", StringComparison.OrdinalIgnoreCase))
                    {
                        html = true;
                    }

                    if(arg.Trim().Equals( "--each-language", StringComparison.OrdinalIgnoreCase))
                    {
                        isTranslationEachLanguage = true;
                    }

                    if (arg.Trim().StartsWith("--from", StringComparison.OrdinalIgnoreCase))
                    {
                        var tokens = arg.Split('=');
                        if(tokens.Length > 1)
                        {
                            from = tokens[1];
                        }
                    }

                    if (arg.Trim().StartsWith("--to", StringComparison.OrdinalIgnoreCase))
                    {
                        var tokens = arg.Split('=');
                        if (tokens.Length > 1)
                        {
                            var temp = tokens[1];
                            var tempTokens = temp.Split(',');

                            if(tempTokens.Length > 0)
                            {
                                toLanguages = tempTokens;
                            }

                        }
                    }
                }
                else
                {
                    var localFileService = host.Services.GetRequiredService<ILocalFileService>();
                    var environment = host.Services.GetRequiredService<IHostEnvironment>();
                    var filePath = Path.GetFullPath(arg, environment.ContentRootPath);

                    //Console.WriteLine($"File: {filePath}");

                    if (localFileService.Exists(filePath))
                    {
                        text = await localFileService.ReadAsync(filePath);
                    }
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
                ToLanguages = toLanguages,
                FromLanguage = from,
                TextType = html ? TextTypes.Html : TextTypes.Plain,
                IsTranslationEachLanguage = isTranslationEachLanguage,
            };

            try
            {
                var result = await translatorService.TranslateAsync(source);

                Console.WriteLine($"Result: {result.ToJson()}");
            }
            catch(InvalidRequestException ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine(ex.GetDetails().ToJson());
            }
            catch(SomethingWrongException ex)
            {
                Console.WriteLine($"Exception: {ex.Message} ");
                Console.WriteLine(ex.GetDetails().ToJson());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
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
                //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All, UnicodeRanges.Cyrillic),
                // ! Caution
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,

            };

            return JsonSerializer.Serialize<T>(obj, actualOptions);
        }
    }
}
