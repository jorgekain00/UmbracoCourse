using Umbraco11.Course.Core.Models;

namespace Umbraco11.Course.Core.Services;

public interface IProductService
{
    List<ProductDTO> GetAll();
}


public class ProductService : IProductService
{
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
}