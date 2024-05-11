using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco11.Course.Core.Repository;
using Umbraco11.Course.Core.Services;
using Umbraco11.Course.Mappings;

namespace Umbraco11.Course.Composer;

public class ProductComposer : IComposer
{
    // example.com/product/1
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.Configure<UmbracoPipelineOptions>(opt =>
            {
                opt.AddFilter(new UmbracoPipelineFilter(
                    "Product integration",
                    applicationBuilder => { },
                    applicationBuilder => { },
                    applicationBuilder =>
                    {
                        applicationBuilder.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllerRoute(
                                "Product custom route",
                                "product/{id}",
                                new
                                {
                                    Controller = "Product",
                                    Action = "Details"
                                });
                        });
                    }
                ));
            });

            builder.WithCollectionBuilder<MapDefinitionCollectionBuilder>()
                .Add<ProductMapping>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
    }
}