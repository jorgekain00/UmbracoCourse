using System.ComponentModel.DataAnnotations;

namespace Umbraco11.Course.Core.Models;
public class ProductCreationItem
{
    [Required]
    public string ProductName { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public List<string> Categories { get; set; }
    [Required]
    public string Description { get; set; }
    public string? SKU { get; set; }
    [Required]
    public string PhotoFileName { get; set; }
    [Required]
    public string Photo { get; set; }
}