using Microsoft.AspNetCore.Mvc;
using Umbraco11.Course.Core.Services;

namespace Umbraco11.Course.ViewComponents;
public class ProductListingViewComponent : ViewComponent
{
    private readonly IProductService productService;

    public ProductListingViewComponent(IProductService productService)
    {
        this.productService = productService;
    }
    public IViewComponentResult Invoke(int number)
    {
        var vm = new ProductListingBlockListViewModel()
        {
            Products = productService.GetUmbracoProducts(number)
        };
        return View(vm);
    }
}