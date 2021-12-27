using AspNetCoreHero.Boilerplate.Application.Abstractions;
using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Application.Common;
using AspNetCoreHero.Boilerplate.Application.Constants;
using AspNetCoreHero.Boilerplate.Application.DTOs.Settings;
using AspNetCoreHero.Boilerplate.Application.Extensions;
using AspNetCoreHero.Boilerplate.Application.Interfaces.Shared;
using AspNetCoreHero.Boilerplate.Web.Services;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Polly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMultiLingualSupport(this IServiceCollection services)
    {
        #region Registering ResourcesPath

        services.AddLocalization(options => options.ResourcesPath = "Resources");

        #endregion Registering ResourcesPath

        services.AddMvc()
           .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
           .AddDataAnnotationsLocalization(options =>
           {
               options.DataAnnotationLocalizerProvider = (type, factory) =>
                   factory.Create(typeof(SharedResource));
           });
        services.AddRouting(o => o.LowercaseUrls = true);
        services.AddHttpContextAccessor();
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var cultures = new List<CultureInfo> {
                    new CultureInfo("en"),
                    new CultureInfo("ar"),
                    new CultureInfo("fr"),
                    new CultureInfo("fa"),
                    new CultureInfo("pt-BR"),
            };
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
            options.SupportedCultures = cultures;
            options.SupportedUICultures = cultures;
        });
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRouting(options => options.LowercaseUrls = true)
            .AddAuthenticationScheme(configuration);
        return services;
    }

    private static IServiceCollection AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        var identityClientSettings = configuration.GetSection(nameof(IdentityClientSettings)).Get<IdentityClientSettings>();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "oidc";

            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                        options =>
                        {
                            options.Cookie.Name = identityClientSettings.IdentityAdminCookieName;
                        })
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = identityClientSettings.IdentityServerBaseUrl;
                options.RequireHttpsMetadata = identityClientSettings.RequireHttpsMetadata;
                options.ClientId = identityClientSettings.ClientId;
                options.ClientSecret = identityClientSettings.ClientSecret;
                options.ResponseType = identityClientSettings.OidcResponseType;
                options.UsePkce = true;

                options.Scope.Clear();
                foreach (string scope in identityClientSettings.Scopes)
                {
                    options.Scope.Add(scope);
                }

                options.ClaimActions.MapJsonKey(identityClientSettings.TokenValidationClaimRole, identityClientSettings.TokenValidationClaimRole, identityClientSettings.TokenValidationClaimRole);

                options.SaveTokens = true;

                options.GetClaimsFromUserInfoEndpoint = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = identityClientSettings.TokenValidationClaimName,
                    RoleClaimType = identityClientSettings.TokenValidationClaimRole
                };

                options.Events = new OpenIdConnectEvents
                {
                    OnMessageReceived = context => OnMessageReceived(context, identityClientSettings),
                    OnRedirectToIdentityProvider = context => OnRedirectToIdentityProvider(context, identityClientSettings)
                };
            });

        // adds user and client access token management
        services.AddAccessTokenManagement(options =>
        {
            // client config is inferred from OpenID Connect settings
            // if you want to specify scopes explicitly, do it here, otherwise the scope parameter will not be sent
            //options.Client = "api";
            options.Client.DefaultClient.Scope = "inventory_management_api";
            options.Client.Clients.Add("identityserver", new ClientCredentialsTokenRequest
            {
                Address = $"{identityClientSettings.IdentityServerBaseUrl}/connect/token",
                ClientId = identityClientSettings.ClientId,
                ClientSecret = identityClientSettings.ClientSecret,
            });
        })
            .ConfigureBackchannelHttpClient()
                .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3)
                }));

        // registers HTTP client that uses the managed user access token
        services.AddUserAccessTokenHttpClient("user_client", configureClient: client =>
        {
            client.DefaultRequestHeaders.AcceptLanguage.Clear();
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture?.TwoLetterISOLanguageName);
            client.BaseAddress = new Uri(configuration[ConfigNames.ApiBaseUrl]);
        });

        return services;
    }

    private static Task OnMessageReceived(MessageReceivedContext context, IdentityClientSettings identityClientSettings)
    {
        context.Properties.IsPersistent = true;
        context.Properties.ExpiresUtc = new DateTimeOffset(DateTime.Now.AddHours(identityClientSettings.IdentityAdminCookieExpiresUtcHours));

        return Task.CompletedTask;
    }

    private static Task OnRedirectToIdentityProvider(RedirectContext context, IdentityClientSettings identityClientSettings)
    {
        if (!string.IsNullOrEmpty(identityClientSettings.IdentityAdminRedirectUri))
        {
            context.ProtocolMessage.RedirectUri = identityClientSettings.IdentityAdminRedirectUri;
        }

        return Task.CompletedTask;
    }

    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityClientSettings>(configuration.GetSection(nameof(IdentityClientSettings)))
            .Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)))
            .Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)))
            .AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
        return services;
    }
}
