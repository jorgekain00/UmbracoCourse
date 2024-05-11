
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco11.Course.Core.Models.Umbraco;

namespace Umbraco11.Course.ViewModels.Umbraco;

public class ProductListingViewModel : Products
{
    public ProductListingViewModel(IPublishedContent content, IPublishedValueFallback publishedValueFallback) : base(content, publishedValueFallback)
    {
    }

    public List<Product> Products { get; set; } = new List<Product>();
}