using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco11.Course.Core.Services;

namespace Umbraco11.Course.Controllers;

public class ProductController : UmbracoPageController, IVirtualPageController
{
    private readonly IProductService productService;
    private readonly IUmbracoContextAccessor umbracoContextAccessor;

    public ProductController(ILogger<UmbracoPageController> logger, ICompositeViewEngine compositeViewEngine,
        IProductService productService, IUmbracoContextAccessor umbracoContextAccessor) : base(logger, compositeViewEngine)
    {
        this.productService = productService;
        this.umbracoContextAccessor = umbracoContextAccessor;
    }
    public IActionResult Details(int id)
    {
        var product = productService.GetAll().FirstOrDefault(x => x.Id == id);

        if (product is null || CurrentPage is null) return NotFound();

        var vm = new ProductViewModel(CurrentPage) { ProductName = product.Name };

        return View(vm);
    }

    public IPublishedContent? FindContent(ActionExecutingContext actionExecutingContext)
    {
        var homepage = umbracoContextAccessor.GetRequiredUmbracoContext()?.Content?.GetAtRoot().FirstOrDefault();

        var productListingPage = homepage?.FirstChildOfType("people");

        return productListingPage ?? homepage;
    }
}
