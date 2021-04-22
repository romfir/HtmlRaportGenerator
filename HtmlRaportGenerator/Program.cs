using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using HtmlRaportGenerator.Tools.ServicesExtensions;

namespace HtmlRaportGenerator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
                          .AddBlazorise(options
                            => options.ChangeTextOnKeyPress = true)
                          .AddBootstrapProviders()
                          .AddFontAwesomeIcons();

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddScoped(sp =>
                    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddGoogleHttpClient();

            builder.Services.AddGoogleAuthentication(builder);

            builder.Services.AddBlazoredLocalStorage(config =>
                    config.JsonSerializerOptions.WriteIndented = true);

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddPreviousStateServices();

            builder.Services.AddMonthStateService();
            builder.Services.AddGoogleDriveService();

            await builder.Build().RunAsync();
        }
    }
}