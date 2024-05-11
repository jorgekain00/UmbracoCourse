
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco11.Course.Core.Models.Umbraco;
using Umbraco11.Course.ViewModels.Umbraco;

namespace Umbraco11.Course.Controllers;

public class ProductsController : RenderController
{
    private readonly IPublishedValueFallback publishedValueFallback;

    public ProductsController(
        ILogger<RenderController> logger,
        ICompositeViewEngine compositeViewEngine,
        IUmbracoContextAccessor umbracoContextAccessor,
        IPublishedValueFallback publishedValueFallback) : base(logger, compositeViewEngine, umbracoContextAccessor)
    {
        this.publishedValueFallback = publishedValueFallback;
    }
    [HttpGet]
    public IActionResult Index(ContentModel model,[FromQuery(Name = "maxprice")] decimal? maxprice)
    {
        var products = (Products)CurrentPage;
        var allProducts = products.Children<Product>();
        if (maxprice is decimal MaxPrice)
        {
            allProducts = allProducts.Where(x=> x.Price <= MaxPrice);
        }

        var vm = new ProductListingViewModel(CurrentPage, publishedValueFallback)
        {
            Products = allProducts.ToList()
        };

        return CurrentTemplate(vm);
    }
}