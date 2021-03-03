using Blazored.LocalStorage;
using Blazored.Modal;
using HtmlRaportGenerator.Tools;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
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
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp =>
                    new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazoredLocalStorage(config =>
                    config.JsonSerializerOptions.WriteIndented = true);

            builder.Services.AddBlazoredModal();

            builder.Services.AddOidcAuthentication(options =>
            {
                //todo appsettings with two enviroments + use secret store for clientId
                options.ProviderOptions.Authority = "https://accounts.google.com/";
                options.ProviderOptions.RedirectUri = "https://localhost:5001/authentication/login-callback";
                options.ProviderOptions.PostLogoutRedirectUri = "https://localhost:5001/authentication/logout-callback";
                options.ProviderOptions.ClientId = "notaclientid";

                options.ProviderOptions.ResponseType = "token id_token";

                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("email");
                options.ProviderOptions.DefaultScopes.Add("openid");
                //todo use library?
                options.ProviderOptions.DefaultScopes.Add(@"https://www.googleapis.com/auth/drive.file");
            });

            builder.Services.AddHttpClient("ServerAPI",
                    client => client.BaseAddress = new Uri("https://www.googleapis.com/"))
                .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

            await builder.Build().RunAsync();
        }
    }
}
