using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreHero.Boilerplate.Web.Abstractions;

public abstract class BaseController<T, TService> : Controller
{
    private ILogger<T> _loggerInstance;
    private IViewRenderService _viewRenderInstance;
    private INotyfService _notifyInstance;
    private TService _serviceInstance;


    protected INotyfService _notify => _notifyInstance ??= HttpContext.RequestServices.GetService<INotyfService>();
    protected TService _service => _serviceInstance ??= HttpContext.RequestServices.GetService<TService>();

    protected ILogger<T> _logger => _loggerInstance ??= HttpContext.RequestServices.GetService<ILogger<T>>();
    protected IViewRenderService _viewRenderer => _viewRenderInstance ??= HttpContext.RequestServices.GetService<IViewRenderService>();
}
