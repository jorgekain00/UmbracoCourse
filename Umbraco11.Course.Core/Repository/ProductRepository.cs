using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Cms.Core.Web;
using Umbraco.Extensions;
using Umbraco11.Course.Core.Models;
using Umbraco11.Course.Core.Models.Umbraco;

namespace Umbraco11.Course.Core.Repository;

public class ProductRepository : IProductRepository
{
    private readonly Guid _productsMediaFolder = Guid.Parse("1ad4e9a3-4ad5-476c-aba2-9602e34e323f");
    private readonly IUmbracoContextFactory umbracoContextFactory;
    private readonly IContentService contentService;
    private readonly IMediaService mediaService;
    private readonly MediaFileManager mediaFileManager;
    private readonly IShortStringHelper shortStringHelper;
    private readonly MediaUrlGeneratorCollection mediaUrlGenerators;
    private readonly IContentTypeBaseServiceProvider contentTypeBaseServiceProvider;
    private readonly IPublishedSnapshotAccessor publishedSnapshotAccessor;

    private readonly string _productNameAlias;
    private readonly string _productPriceAlias;
    private readonly string _productCategoriesAlias;
    private readonly string _productDescriptionAlias;
    private readonly string _productSkuAlias;
    private readonly string _productPhotosAlias;

    public ProductRepository(IUmbracoContextFactory umbracoContextFactory,
        IContentService contentService,
        IMediaService mediaService,
        MediaFileManager mediaFileManager,
        IShortStringHelper shortStringHelper,
        MediaUrlGeneratorCollection mediaUrlGenerators,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
        IPublishedSnapshotAccessor publishedSnapshotAccessor)
    {
        this.umbracoContextFactory = umbracoContextFactory;
        this.contentService = contentService;
        this.mediaService = mediaService;
        this.mediaFileManager = mediaFileManager;
        this.shortStringHelper = shortStringHelper;
        this.mediaUrlGenerators = mediaUrlGenerators;
        this.contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        this.publishedSnapshotAccessor = publishedSnapshotAccessor;

        _productNameAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.ProductName).Alias;
        _productPriceAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.Price).Alias;
        _productCategoriesAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.Category).Alias;
        _productDescriptionAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.Description).Alias;
        _productSkuAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.Sku).Alias;
        _productPhotosAlias = Product.GetModelPropertyType(publishedSnapshotAccessor, x => x.Photos).Alias;
    }
    private GuidUdi? CreateProductImage(string fileName, string photo)
    {
        //Save image to a temp path
        var tmpFilePath = Path.GetTempFileName();
        using var image = SixLabors.ImageSharp.Image.Load(Convert.FromBase64String(photo));
        image.Save(tmpFilePath, new JpegEncoder());
        //load file into a filestream
        var fileInfo = new FileInfo(tmpFilePath);
        var filestream = fileInfo.OpenReadWithRetry();
        if (filestream is null)
        {
            throw new InvalidOperationException("Could not acquire file stream");
        }

        var umbracoMedia = mediaService.CreateMedia(fileName, _productsMediaFolder, Constants.Conventions.MediaTypes.Image);

        using (filestream)
        {
            umbracoMedia.SetValue(mediaFileManager, mediaUrlGenerators, shortStringHelper, contentTypeBaseServiceProvider,
            Constants.Conventions.Media.File
            , fileName, filestream, null, null);
            var result = mediaService.Save(umbracoMedia);

            if (!result.Success) { return null; }
        }
        return umbracoMedia.GetUdi();
    }

    public Product Get(int id)
    {
        using var cref = umbracoContextFactory.EnsureUmbracoContext();
        var content = cref.UmbracoContext.Content.GetById(id);
        return (content as Product);
    }

    public Product Create(ProductCreationItem productCreationItem)
    {
        var productImage = CreateProductImage(productCreationItem.PhotoFileName, productCreationItem.Photo);
        if (productImage is null) return null;

        var productsRoot = GetProductsRootPage();

        var productContent = contentService.Create(productCreationItem.ProductName, productsRoot.Key, Product.ModelTypeAlias);

        productContent.SetValue(_productNameAlias, productCreationItem.ProductName);
        productContent.SetValue(_productPriceAlias, productCreationItem.Price);
        productContent.SetValue(_productCategoriesAlias, string.Join(",", productCreationItem.Categories));
        productContent.SetValue(_productDescriptionAlias, productCreationItem.Description);
        productContent.SetValue(_productSkuAlias, productCreationItem.SKU);
        productContent.SetValue(_productPhotosAlias, productImage);

        contentService.SaveAndPublish(productContent);

        return Get(productContent.Id);
    }

    public bool Delete(int id)
    {
        var product = contentService.GetById(id);
        if (product is null)
        {
            return false;
        }
        else
        {
            var operationResult = contentService.Delete(product);
            return operationResult.Success;
        }
    }

    public List<Product> GetProducts(string productSKU, decimal? maxPrice)
    {
        var products = GetProductsRootPage();
        var final = new List<Product>();

        if (products is Products productsRoot)
        {
            var filteredProducts = productsRoot.Children<Product>();
            if (!string.IsNullOrEmpty(productSKU))
            {
                filteredProducts = filteredProducts.Where(x => x.Sku.InvariantEquals(productSKU));
            }
            if (maxPrice is decimal mprice)
            {
                filteredProducts = filteredProducts.Where(x => x.Price <= mprice);
            }
            final = filteredProducts?.ToList() ?? new List<Product>();
        }
        return final;
    }
    private Products? GetProductsRootPage()
    {
        using var cref = umbracoContextFactory.EnsureUmbracoContext();
        var rootNode = cref?.UmbracoContext?.Content?
            .GetAtRoot().FirstOrDefault(x => x.ContentType.Alias == Home.ModelTypeAlias);
        return rootNode?.Descendant<Products>();
    }

    public Product Update(int id, ProductUpdateItem productUpdateItem)
    {
        var productContent = contentService.GetById(id);
        if (!string.IsNullOrEmpty(productUpdateItem.ProductName))
        {
            productContent.SetValue(_productNameAlias, productUpdateItem.ProductName);
        }
        if (!string.IsNullOrEmpty(productUpdateItem.Description))
        {
            productContent.SetValue(_productDescriptionAlias, productUpdateItem.Description);
        }
        if (productUpdateItem.Categories != null && productUpdateItem.Categories.Any())
        {
            productContent.SetValue(_productCategoriesAlias, string.Join(",", productUpdateItem.Categories));
        }
        if (!string.IsNullOrEmpty(productUpdateItem.SKU))
        {
            productContent.SetValue(_productSkuAlias, productUpdateItem.SKU);
        }
        if (!string.IsNullOrEmpty(productUpdateItem.PhotoFileName) && !string.IsNullOrEmpty(productUpdateItem.Photo))
        {
            var productImage = CreateProductImage(productUpdateItem.PhotoFileName, productUpdateItem.Photo);
            if (productImage != null)
            {
                productContent.SetValue(_productPhotosAlias, productImage);
            }
        }

        contentService.SaveAndPublish(productContent);

        return Get(id);
    }
}

public interface IProductRepository
{
    public List<Product> GetProducts(string productSKU, decimal? maxPrice);
    bool Delete(int id);
    Product Create(ProductCreationItem productCreationItem);
    Product Get(int id);
    Product Update(int id, ProductUpdateItem productUpdateItem);
}