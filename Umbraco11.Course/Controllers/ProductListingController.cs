using Microsoft.AspNetCore.Mvc;
using NPoco.Expressions;
using StackExchange.Profiling.Internal;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.UmbracoContext;
using Umbraco11.Course.Core.Models.Umbraco;
using Umbraco11.Course.Core.Services;

namespace Umbraco11.Course.Controllers.BackOffice;

public class ProductListingController : UmbracoAuthorizedApiController
{
    // /umbraco/backoffice/api/ProductListing/GetProducts?number=x
    private readonly IUmbracoContextFactory umbracoContextFactory;
    private readonly IProductService productService;

    public ProductListingController(IProductService productService)
    {
        this.productService = productService;
    }
    public IActionResult GetProducts(int number){
       var products = productService.GetUmbracoProducts(number);
        return Ok(products);
    }

}
