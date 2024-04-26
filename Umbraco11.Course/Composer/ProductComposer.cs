using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Web.Common.ApplicationBuilder;
using Umbraco11.Course.Core.Services;

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
    }
}