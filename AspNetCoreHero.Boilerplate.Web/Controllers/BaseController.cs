using AspNetCoreHero.Boilerplate.Web.Abstractions;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreHero.Boilerplate.Web.Controllers;

public abstract class BaseController<T> : Controller
{
    private ILogger<T> _loggerInstance;
    private IViewRenderService _viewRenderInstance;
    private INotyfService _notifyInstance;
    protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();

    protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    protected IViewRenderService _viewRenderer => _viewRenderInstance ??= HttpContext.RequestServices.GetService<IViewRenderService>();
}
