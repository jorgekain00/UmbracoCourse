using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Umbraco11.Course.Core.Models;
using Umbraco11.Course.Core.Models.Records;
using Umbraco11.Course.Core.Models.Umbraco;

namespace Umbraco11.Course.Core.Services;

public interface IProductService
{
    List<ProductDTO> GetAll();
    List<ProductResponseItem> GetUmbracoProducts(int number);
}


public class ProductService : IProductService
{
    private readonly IUmbracoContextFactory umbracoContextFactory;

    public ProductService(IUmbracoContextFactory umbracoContextFactory)
    {
        this.umbracoContextFactory = umbracoContextFactory;
    }
    public List<ProductDTO> GetAll()
    {
        return new List<ProductDTO>{
            new ProductDTO{Id = 1, Name = "Product Name 1"},
            new ProductDTO{Id = 2, Name = "Product Name 2"},
            new ProductDTO{Id = 3, Name = "Product Name 3"},
            new ProductDTO{Id = 4, Name = "Product Name 4"},
            new ProductDTO{Id = 5, Name = "Product Name 5"},
       };
    }

    public List<ProductResponseItem> GetUmbracoProducts(int number)
    {
        var final = new List<ProductResponseItem>();

        using var cred = umbracoContextFactory.EnsureUmbracoContext();
        var contentCache = cred.UmbracoContext.Content;

        var products = contentCache
         ?.GetAtRoot()
         ?.FirstOrDefault(x => x.ContentType.Alias == Home.ModelTypeAlias)
         ?.Descendant<Products>()
         ?.Children<Product>()
         ?.Take(number);

        if (products is not null && products.Any())
        {
            final = products.Select(x => new ProductResponseItem(x.Id, x?.ProductName ?? x.Name, x?.Photos?.Url() ?? "#")).ToList();
        }
        return final;
    }
}