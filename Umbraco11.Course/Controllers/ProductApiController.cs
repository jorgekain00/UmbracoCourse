using Microsoft.AspNetCore.Mvc;
using NPoco.Expressions;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco11.Course.Core.Models;
using Umbraco11.Course.Core.Models.Umbraco;
using Umbraco11.Course.Core.Repository;
using Umbraco11.Course.ViewModels.DTO;

namespace Umbraco11.Course.Controllers;

///umbraco/api/productapi/{action}
// [Route("api/products")]
public class ProductApiController : UmbracoApiController
{
    private readonly IProductRepository productRepository;
    private readonly IUmbracoMapper umbracoMapper;

    public record ProductReadRequest(string productSKU, decimal? maxPrice);
    public ProductApiController(IProductRepository productRepository, IUmbracoMapper umbracoMapper)
    {
        this.productRepository = productRepository;
        this.umbracoMapper = umbracoMapper;
    }
    [HttpGet("api/products")]
    public IActionResult Read([FromQuery] ProductReadRequest productReadRequest)
    {
        var mapped = umbracoMapper.MapEnumerable<Product, ProductApiDTO>(productRepository.GetProducts(productReadRequest.productSKU, productReadRequest.maxPrice));
        return Ok(mapped);
    }

    [HttpDelete("api/products/{id:int}")]
    public IActionResult Delete(int id)
    {
        var result = productRepository.Delete(id);
        if (result)
        {
            return Ok();
        }
        return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting product with id {id}");
    }
    [HttpPost("api/products")]
    public IActionResult Create([FromBody] ProductCreationItem productCreationItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Fields error");
        }

        var product = productRepository.Create(productCreationItem);

        if (product is null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Error creating product");
        }

        return Ok(umbracoMapper.Map<Product, ProductApiDTO>(product));
    }
    [HttpPut("api/products/{id:int}")]
    public IActionResult Update(int id, [FromBody] ProductUpdateItem productUpdateItem)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (productRepository.Get(id) is null)
        {
            return NotFound();
        }

        var product = productRepository.Update(id, productUpdateItem);
        return (product is null)
            ? StatusCode(StatusCodes.Status500InternalServerError, $"Error updating product {id}")
            : Ok(umbracoMapper.Map<Product, ProductApiDTO>(product));
    }
}