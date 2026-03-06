using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace ExampleImplementation.Repositories.Services;

public sealed class ProductCatalogService : IProductCatalogService
{
    private const int DefaultPageSize = 10;
    private const int MaximumPageSize = 100;

    private readonly IProductCatalogRepository _productCatalogRepository;

    public ProductCatalogService(IProductCatalogRepository productCatalogRepository)
    {
        _productCatalogRepository = productCatalogRepository;
    }

    public async Task<PagingResultsModel<ExampleProduct>> SearchAsync(ProductSearchRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var pageIndex = Math.Max(0, request.PageIndex);
        var pageSize = request.PageSize <= 0
            ? DefaultPageSize
            : Math.Min(request.PageSize, MaximumPageSize);

        var normalizedSearch = request.SearchText?.Trim() ?? string.Empty;
        var data = await _productCatalogRepository.ListAsync(cancellationToken);

        var query = data.AsEnumerable();
        if (!request.IncludeInactive)
        {
            query = query.Where(product => product.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(product =>
                product.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                product.Category.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
        }

        var ordered = query.OrderBy(product => product.Name).ThenBy(product => product.Id);
        var totalRecordCount = ordered.Count();
        var pageResults = ordered
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToList();

        var paging = new PagingModel(pageIndex, pageSize);
        return new PagingResultsModel<ExampleProduct>(paging, totalRecordCount, pageResults);
    }

    public async Task<IReadOnlyList<ExampleProduct>> FeaturedAsync(int take = 3, CancellationToken cancellationToken = default)
    {
        var normalizedTake = take <= 0 ? 3 : Math.Min(take, 10);
        var data = await _productCatalogRepository.ListAsync(cancellationToken);

        return data
            .Where(product => product.IsActive)
            .OrderByDescending(product => product.Price)
            .ThenBy(product => product.Name)
            .Take(normalizedTake)
            .ToList();
    }
}
