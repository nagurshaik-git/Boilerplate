using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Application.Constants;
using AspNetCoreHero.Boilerplate.Application.DTOs.Settings;
using FluentValidation;
using IdentityModel.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace AspNetCoreHero.Boilerplate.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
    {
        var identityClientSettings = configuration.GetSection(nameof(IdentityClientSettings)).Get<IdentityClientSettings>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        //services.AddAccessTokenManagement(options =>
        //{
        //    options.Client.Clients.Add("identityserver", new ClientCredentialsTokenRequest
        //    {
        //        Address = $"{identityClientSettings.IdentityServerBaseUrl}/connect/token",
        //        ClientId = identityClientSettings.ClientId,
        //        ClientSecret = identityClientSettings.ClientSecret,
        //        Scope = ""
        //    });
        //});

        //services.AddHttpClient<BrandsClient>(client =>
        //{
        //    client.BaseAddress = new Uri(configuration[ConfigNames.ApiBaseUrl]);
        //})
        //    .AddUserAccessTokenHandler();


        //services//.AutoRegisterInterfaces<IApiService>()
        //    .AddHttpClient("Inventory.API", client =>
        //    {
        //        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        //        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
        //        client.BaseAddress = new Uri(configuration[ConfigNames.ApiBaseUrl]);
        //    })
        //    .AddClientAccessTokenHandler()
        //    .Services
        //    .AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Inventory.API"));

        //services.AddHttpClient<BrandsClient>(client =>
        //{
        //    client.DefaultRequestHeaders.AcceptLanguage.Clear();
        //    client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
        //    client.BaseAddress = new Uri(configuration[ConfigNames.ApiBaseUrl]);
        //}).AddClientAccessTokenHandler()
        //.Services
        //.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient());

        return services;
    }


    public static IServiceCollection AutoRegisterInterfaces<T>(this IServiceCollection services)
    {
        var @interface = typeof(T);

        var types = @interface
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterface($"I{t.Name}"),
                Implementation = t
            })
            .Where(t => t.Service != null);

        foreach (var type in types)
        {
            if (@interface.IsAssignableFrom(type.Service))
            {
                services.AddTransient(type.Service, type.Implementation);
            }
        }

        return services;
    }
}
