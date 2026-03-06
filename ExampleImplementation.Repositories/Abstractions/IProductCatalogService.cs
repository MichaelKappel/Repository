using ExampleImplementation.Repositories.Models;
using MichaelKappel.Repositories.Common.Models;

namespace ExampleImplementation.Repositories.Abstractions;

public interface IProductCatalogService
{
    Task<PagingResultsModel<ExampleProduct>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExampleProduct>> FeaturedAsync(int take = 3, CancellationToken cancellationToken = default);
}
