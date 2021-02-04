using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Microsoft.Extensions.Options;
using System.IO;

namespace Sample.Azure.Translator.App
{
    class Program
    {
        public static Task Main(string[] args)
        {
            using var host = CreateHostBuilder(args).Build();

            RunProcess(host);

            return host.RunAsync();
        }

        private static Task RunProcess(IHost host)
        {
            // code here

            return Task.CompletedTask;
        }

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            // DI here

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
            var actualOptions = options ?? new JsonSerializerOptions { WriteIndented = true };

            return JsonSerializer.Serialize<T>(obj, actualOptions);
        }
    }
}
