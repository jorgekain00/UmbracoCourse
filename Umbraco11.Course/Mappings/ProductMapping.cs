using Umbraco.Cms.Core.Mapping;
using Umbraco11.Course.Core.Models.Umbraco;
using Umbraco11.Course.ViewModels.DTO;

namespace Umbraco11.Course.Mappings;
public class ProductMapping : IMapDefinition
{
    public void DefineMaps(IUmbracoMapper mapper)
    {
        mapper.Define<Product, ProductApiDTO>((source,context)=> new ProductApiDTO(),MapProduct);
    }

    private void MapProduct(Product source, ProductApiDTO target, MapperContext context)
    {
        target.Id = source.Id;
        target.ProductName = source?.ProductName ?? source.Name;
        target.Price = source.Price;
        target.ImageUrl = source.Photos?.Url() ?? "#";
        target.ProductSKU = source?.Sku ?? string.Empty;
        target.Categories = source?.Category.ToList() ?? new List<string>();
        target.Description = source?.Description ?? string.Empty;
    }
}