using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Web.Areas.Catalog.Controllers;

[Area("Catalog")]
public class BrandController : BaseController<BrandController, IBrandsClient>
{
    public IActionResult Index()
    {
        var model = new BrandDto();
        return View(model);
    }

    public async Task<IActionResult> LoadAll()
    {
        var response = await _service.SearchAsync(new BrandListFilter() { PageSize = 10 });
        if (response.Succeeded)
        {
            return PartialView("_ViewAll", response.Data);
        }
        return PartialView("_ViewAll", new List<BrandDto>());
    }

    public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
    {
        //var brandsResponse = await _mediator.Send(new GetAllBrandsCachedQuery());

        //if (id == 0)
        //{
        //    var BrandDto = new BrandDto();
        //    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", BrandDto) });
        //}
        //else
        //{
        //    var response = await _mediator.Send(new GetBrandByIdQuery() { Id = id });
        //    if (response.Succeeded)
        //    {
        //        var BrandDto = _mapper.Map<BrandDto>(response.Data);
        //        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", BrandDto) });
        //    }
        //    return null;
        //}
        var BrandDto = new BrandDto();
        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", BrandDto) });
    }

    [HttpPost]
    public async Task<JsonResult> OnPostCreateOrEdit(int id, BrandDto brand)
    {
        //if (ModelState.IsValid)
        //{
        //    if (id == 0)
        //    {
        //        var createBrandCommand = _mapper.Map<CreateBrandCommand>(brand);
        //        var result = await _mediator.Send(createBrandCommand);
        //        if (result.Succeeded)
        //        {
        //            id = result.Data;
        //            _notify.Success($"Brand with ID {result.Data} Created.");
        //        }
        //        else _notify.Error(result.Message);
        //    }
        //    else
        //    {
        //        var updateBrandCommand = _mapper.Map<UpdateBrandCommand>(brand);
        //        var result = await _mediator.Send(updateBrandCommand);
        //        if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
        //    }
        //    var response = await _mediator.Send(new GetAllBrandsCachedQuery());
        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<BrandDto>>(response.Data);
        //        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //        return new JsonResult(new { isValid = true, html = html });
        //    }
        //    else
        //    {
        //        _notify.Error(response.Message);
        //        return null;
        //    }
        //}
        //else
        //{
        //    var html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", brand);
        //    return new JsonResult(new { isValid = false, html = html });
        //}

        string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new List<BrandDto>());
        return new JsonResult(new { isValid = true, html = html });
    }

    [HttpPost]
    public async Task<JsonResult> OnPostDelete(int id)
    {
        //var deleteCommand = await _mediator.Send(new DeleteBrandCommand { Id = id });
        //if (deleteCommand.Succeeded)
        //{
        //    _notify.Information($"Brand with Id {id} Deleted.");
        //    var response = await _mediator.Send(new GetAllBrandsCachedQuery());
        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<BrandDto>>(response.Data);
        //        var html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //        return new JsonResult(new { isValid = true, html = html });
        //    }
        //    else
        //    {
        //        _notify.Error(response.Message);
        //        return null;
        //    }
        //}
        //else
        //{
        //    _notify.Error(deleteCommand.Message);
        //    return null;
        //}

        string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new List<BrandDto>());
        return new JsonResult(new { isValid = true, html });
    }
}
