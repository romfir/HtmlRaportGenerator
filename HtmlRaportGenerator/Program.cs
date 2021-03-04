using Blazored.LocalStorage;
using Blazored.Modal;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using HtmlRaportGenerator.Services;
using HtmlRaportGenerator.Tools;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HtmlRaportGenerator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services
                          .AddBlazorise(options =>
                          {
                              options.ChangeTextOnKeyPress = true;
                          })
                          .AddBootstrapProviders()
                          .AddFontAwesomeIcons();


            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp =>
                    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage(config =>
                    config.JsonSerializerOptions.WriteIndented = true);

            builder.Services.AddBlazoredModal();

            builder.Services.AddOptions();
            builder.Services.AddAuthorizationCore();

            builder.Services.AddOidcAuthentication(options =>
                builder.Configuration.Bind("Google", options.ProviderOptions)
            );

            builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

            builder.Services.AddHttpClient(StaticHelpers.HttpClientName,
                    client => client.BaseAddress = new Uri("https://www.googleapis.com/"))
                .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient(StaticHelpers.HttpClientName));

            builder.Services.AddScoped<MonthStateService>();
            builder.Services.AddScoped<GoogleDriveService>();

            await builder.Build().RunAsync();
        }
    }
}
