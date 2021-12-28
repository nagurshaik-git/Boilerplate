using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Web.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
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

    public async Task<JsonResult> OnGetCreateOrEdit(Guid? id = null)
    {
        if (id == null)
        {
            var brand = new BrandDto();
            return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", brand) });
        }
        else
        {
            var response = await _service.GetAsync(id ?? Guid.Empty);
            if (response.Succeeded)
            {
                return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", response.Data) });
            }
            return null;
        }
    }

    [HttpPost]
    public async Task<JsonResult> OnPostCreateOrEdit(Guid id, CreateBrandRequest createBrandRequest)
    {
        if (ModelState.IsValid)
        {
            if (id == Guid.Empty)
            {
                var result = await _service.CreateAsync(createBrandRequest);
                if (result.Succeeded)
                {
                    id = result.Data;
                    _notify.Success($"Brand with ID {result.Data} Created.");
                }
                else _notify.Error(string.Join(",", result.Messages));
            }
            else
            {
                var result = await _service.UpdateAsync(id, new UpdateBrandRequest()
                {
                    Description = createBrandRequest.Description,
                    Name = createBrandRequest.Name,
                });
                if (result.Succeeded) _notify.Information($"Brand with ID {result.Data} Updated.");
            }

            var response = await _service.SearchAsync(new BrandListFilter() { PageSize = 10 });
            if (response.Succeeded)
            {
                string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", response.Data);
                return new JsonResult(new { isValid = true, html = html });
            }
            else
            {
                _notify.Error(string.Join(",", response.Messages));
                return null;
            }
        }
        else
        {
            string html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", createBrandRequest);
            return new JsonResult(new { isValid = false, html = html });
        }
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
