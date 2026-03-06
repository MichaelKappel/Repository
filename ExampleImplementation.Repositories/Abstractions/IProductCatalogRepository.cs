using ExampleImplementation.Repositories.Models;

namespace ExampleImplementation.Repositories.Abstractions;

public interface IProductCatalogRepository
{
    Task<IReadOnlyList<ExampleProduct>> ListAsync(CancellationToken cancellationToken = default);
}
