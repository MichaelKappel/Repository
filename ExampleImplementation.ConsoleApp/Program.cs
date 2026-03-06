using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;
using ExampleImplementation.Repositories.Repositories;
using ExampleImplementation.Repositories.Services;

namespace ExampleImplementation.ConsoleApp;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IProductCatalogRepository repository = new InMemoryProductCatalogRepository();
        IProductCatalogService service = new ProductCatalogService(repository);

        var searchText = args.FirstOrDefault() ?? "tool";
        var searchRequest = new ProductSearchRequest(searchText, pageIndex: 0, pageSize: 5);

        var page = await service.SearchAsync(searchRequest);
        Console.WriteLine($"Search '{searchText}' returned {page.TotalRecordCount} active products. Showing page {page.PageIndex + 1} of {page.PageCount}.");

        foreach (var product in page.Results)
        {
            Console.WriteLine($"- {product.Name} [{product.Category}] ${product.Price:F2}");
        }

        var featured = await service.FeaturedAsync(3);
        Console.WriteLine();
        Console.WriteLine("Featured products:");
        foreach (var product in featured)
        {
            Console.WriteLine($"* {product.Name} (${product.Price:F2})");
        }
    }
}
