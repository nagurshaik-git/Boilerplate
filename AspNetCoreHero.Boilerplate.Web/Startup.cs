using AspNetCoreHero.Boilerplate.Application.Common;
using AspNetCoreHero.Boilerplate.Application.Extensions;
using AspNetCoreHero.Boilerplate.Web.Abstractions;
using AspNetCoreHero.Boilerplate.Web.Extensions;
using AspNetCoreHero.Boilerplate.Web.Permission;
using AspNetCoreHero.Boilerplate.Web.Services;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace AspNetCoreHero.Boilerplate.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddNotyf(o =>
        {
            o.DurationInSeconds = 10;
            o.IsDismissable = true;
            o.HasRippleEffect = true;
        });

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddSharedInfrastructure(Configuration)
            .AddInfrastructure(Configuration)
            .AddApplicationLayer(Configuration)
            .AddMultiLingualSupport()
            .AddDistributedMemoryCache()
            .AddTransient<IActionContextAccessor, ActionContextAccessor>()
            .AddScoped<IViewRenderService, ViewRenderService>()
            .AddControllersWithViews()
            .AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AutoRegisterInterfaces<IApiService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }
        app.UseNotyf();
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseMultiLingualFeature();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{area=Dashboard}/{controller=Home}/{action=Index}/{id?}")
            ;
        });
    }
}
