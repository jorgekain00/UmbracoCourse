using Umbraco11.Course.Core.Models.Records;

namespace Umbraco11.Course.ViewModels;

public class ProductListingBlockListViewModel
{
    public List<ProductResponseItem> Products { get; set; } = new();
}