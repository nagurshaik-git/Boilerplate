using AspNetCoreHero.Boilerplate.Application.DTOs.Settings;
using AspNetCoreHero.Boilerplate.Application.Interfaces.Shared;
using AspNetCoreHero.Boilerplate.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMultiLingualSupport(this IServiceCollection services)
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
    }

    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        //services.AddPersistenceContexts(configuration);
        services.AddAuthenticationScheme(configuration);
    }

    private static void AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

        var identityClientSettings = configuration.GetSection(nameof(IdentityClientSettings)).Get<IdentityClientSettings>();

        //services
        //        .AddIdentity<ApplicationUser, Applicationr>()
        //        .AddDefaultTokenProviders();

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
        services.AddMvc(o =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            o.Filters.Add(new AuthorizeFilter(policy));
        });
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

    //private static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
    //{
    //    if (configuration.GetValue<bool>("UseInMemoryDatabase"))
    //    {
    //        services.AddDbContext<IdentityContext>(options =>
    //            options.UseInMemoryDatabase("IdentityDb"));
    //        services.AddDbContext<ApplicationDbContext>(options =>
    //            options.UseInMemoryDatabase("ApplicationDb"));
    //    }
    //    else
    //    {
    //        services.AddDbContext<IdentityContext>(options => options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));
    //        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ApplicationConnection")));
    //    }
    //    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    //    {
    //        options.SignIn.RequireConfirmedAccount = true;
    //        options.Password.RequireNonAlphanumeric = false;
    //    }).AddEntityFrameworkStores<IdentityContext>().AddDefaultUI().AddDefaultTokenProviders();
    //}

    public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<IdentityClientSettings>(configuration.GetSection(nameof(IdentityClientSettings)));
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));
        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));
        services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
    }
}
