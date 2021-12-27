using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Web.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreHero.Boilerplate.Web.Areas.Dashboard.Controllers;

[Area("Dashboard")]
public class HomeController : BaseController<HomeController, IStatsClient>
{
    public IActionResult Index()
    {
        _notify.Information("Welcome to 6Storage inventory!");
        return View();
    }
}
