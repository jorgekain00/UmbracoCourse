using Umbraco11.Course.Core.Models.Records;

namespace Umbraco11.Course.ViewModels;

public class ProductListingViewModel
{
    public List<ProductResponseItem> Products { get; set; } = new();
}