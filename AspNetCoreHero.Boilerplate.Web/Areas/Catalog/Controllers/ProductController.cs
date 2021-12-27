using AspNetCoreHero.Boilerplate.Application.ApiService;
using AspNetCoreHero.Boilerplate.Application.Constants;
using AspNetCoreHero.Boilerplate.Web.Abstractions;
using AspNetCoreHero.Boilerplate.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreHero.Boilerplate.Web.Areas.Catalog.Controllers;

[Area("Catalog")]
public class ProductController : BaseController<ProductController, IProductsClient>
{
    public IActionResult Index()
    {
        var model = new ProductDto();
        return View(model);
    }

    public async Task<IActionResult> LoadAll()
    {
        var response = await _service.SearchAsync(new ProductListFilter() { PageSize = 10 });
        if (response.Succeeded)
        {
            return PartialView("_ViewAll", response.Data);
        }
        return PartialView("_ViewAll", new List<ProductDto>());
    }

    [Authorize(Policy = Permissions.Users.View)]
    public async Task<JsonResult> OnGetCreateOrEdit(int id = 0)
    {
        var ProductDto = new ProductDto();
        //var brandsResponse = await _mediator.Send(new GetAllBrandsCachedQuery());

        //if (id == 0)
        //{
        //    var ProductDto = new ProductDto();
        //    if (brandsResponse.Succeeded)
        //    {
        //        var brandViewModel = _mapper.Map<List<BrandViewModel>>(brandsResponse.Data);
        //        ProductDto.Brands = new SelectList(brandViewModel, nameof(BrandViewModel.Id), nameof(BrandViewModel.Name), null, null);
        //    }
        //    return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ProductDto) });
        //}
        //else
        //{
        //    var response = await _mediator.Send(new GetProductByIdQuery() { Id = id });
        //    if (response.Succeeded)
        //    {
        //        var ProductDto = _mapper.Map<ProductDto>(response.Data);
        //        if (brandsResponse.Succeeded)
        //        {
        //            var brandViewModel = _mapper.Map<List<BrandViewModel>>(brandsResponse.Data);
        //            ProductDto.Brands = new SelectList(brandViewModel, nameof(BrandViewModel.Id), nameof(BrandViewModel.Name), null, null);
        //        }
        //        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ProductDto) });
        //    }
        //    return null;
        //}

        return new JsonResult(new { isValid = true, html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", ProductDto) });
    }

    [HttpPost]
    public async Task<JsonResult> OnPostCreateOrEdit(int id, ProductDto product)
    {
        if (ModelState.IsValid)
        {
            if (id == 0)
            {
                //var createProductCommand = _mapper.Map<CreateProductCommand>(product);
                //var result = await _mediator.Send(createProductCommand);
                //if (result.Succeeded)
                //{
                //    id = result.Data;
                //    _notify.Success($"Product with ID {result.Data} Created.");
                //}
                //else _notify.Error(result.Message);
            }
            else
            {
                //var updateProductCommand = _mapper.Map<UpdateProductCommand>(product);
                //var result = await _mediator.Send(updateProductCommand);
                //if (result.Succeeded) _notify.Information($"Product with ID {result.Data} Updated.");
            }
            if (Request.Form.Files.Count > 0)
            {
                //var file = Request.Form.Files.FirstOrDefault();
                //byte[] image = file.OptimizeImageSize(700, 700);
                //await _mediator.Send(new UpdateProductImageCommand() { Id = id, Image = image });
            }
            //var response = await _mediator.Send(new GetAllProductsCachedQuery());
            //if (response.Succeeded)
            //{
            //    var viewModel = _mapper.Map<List<ProductDto>>(response.Data);
            //    string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
            //    return new JsonResult(new { isValid = true, html = html });
            //}
            //else
            //{
            //    _notify.Error(response.Message);
            //    return null;
            //}
            string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new List<ProductDto>());
            return new JsonResult(new { isValid = true, html = html });
        }
        else
        {
            string html = await _viewRenderer.RenderViewToStringAsync("_CreateOrEdit", product);
            return new JsonResult(new { isValid = false, html = html });
        }
    }

    [HttpPost]
    public async Task<JsonResult> OnPostDelete(int id)
    {
        //var deleteCommand = await _mediator.Send(new DeleteProductCommand { Id = id });
        //if (deleteCommand.Succeeded)
        //{
        //    _notify.Information($"Product with Id {id} Deleted.");
        //    var response = await _mediator.Send(new GetAllProductsCachedQuery());
        //    if (response.Succeeded)
        //    {
        //        var viewModel = _mapper.Map<List<ProductDto>>(response.Data);
        //        string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", viewModel);
        //        return new JsonResult(new { isValid = true, html });
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

        string html = await _viewRenderer.RenderViewToStringAsync("_ViewAll", new List<ProductDto>());
        return new JsonResult(new { isValid = true, html });
    }
}
