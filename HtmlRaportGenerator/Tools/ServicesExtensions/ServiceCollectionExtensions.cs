using HtmlRaportGenerator.Models;
using HtmlRaportGenerator.Tools.StateHelper.Implementations;
using HtmlRaportGenerator.Tools.StateHelper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using HtmlRaportGenerator.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace HtmlRaportGenerator.Tools.ServicesExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGoogleHttpClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(StaticHelpers.HttpClientName,
                client => client.BaseAddress = new Uri("https://www.googleapis.com/"))
            .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

        serviceCollection.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
            .CreateClient(StaticHelpers.HttpClientName));

        return serviceCollection;
    }

    public static IServiceCollection AddPreviousStateServices(this IServiceCollection serviceCollection)
        => serviceCollection.AddScoped(typeof(IPreviousState<HourWithQuarter>), typeof(PreviousStateService<HourWithQuarter>))
            .AddScoped(typeof(IPreviousState<ICollection<Day>>), typeof(PreviousStateCollectionService<Day>));

    public static IServiceCollection AddGoogleDriveService(this IServiceCollection serviceCollection)
        => serviceCollection.AddScoped<GoogleDriveService>();

    public static IServiceCollection AddGoogleAuthentication(this IServiceCollection serviceCollection, WebAssemblyHostBuilder builder)
    {
        serviceCollection.AddOidcAuthentication(options =>
            builder.Configuration.Bind("Google", options.ProviderOptions)
        );

        serviceCollection.AddScoped<CustomAuthorizationMessageHandler>();

        return serviceCollection;
    }

    public static IServiceCollection AddMonthStateService(this IServiceCollection serviceCollection)
        => serviceCollection.AddScoped<MonthStateService>();
}