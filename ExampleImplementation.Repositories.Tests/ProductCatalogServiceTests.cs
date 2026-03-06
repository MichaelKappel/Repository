using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;
using ExampleImplementation.Repositories.Services;

namespace ExampleImplementation.Repositories.Tests;

[TestClass]
public class ProductCatalogServiceTests
{
    private sealed class FakeRepository : IProductCatalogRepository
    {
        private readonly IReadOnlyList<ExampleProduct> _data;

        public FakeRepository(IReadOnlyList<ExampleProduct> data)
        {
            _data = data;
        }

        public Task<IReadOnlyList<ExampleProduct>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_data);
        }
    }

    private static IReadOnlyList<ExampleProduct> BuildProducts()
    {
        return new List<ExampleProduct>
        {
            new() { Id = 1, Name = "Hammer", Category = "Tools", Price = 19.99m, IsActive = true },
            new() { Id = 2, Name = "Handsaw", Category = "Tools", Price = 24.99m, IsActive = true },
            new() { Id = 3, Name = "Helmet", Category = "Safety", Price = 39.99m, IsActive = false },
            new() { Id = 4, Name = "Drill", Category = "Power", Price = 129.99m, IsActive = true },
            new() { Id = 5, Name = "Hammer Drill", Category = "Power", Price = 159.99m, IsActive = true }
        };
    }

    [TestMethod]
    public async Task SearchAsync_FiltersBySearchText_AndExcludesInactiveByDefault()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.SearchAsync(new ProductSearchRequest("hammer", pageIndex: 0, pageSize: 10));

        Assert.AreEqual(2, result.TotalRecordCount);
        CollectionAssert.AreEqual(new[] { "Hammer", "Hammer Drill" }, result.Results.Select(product => product.Name).ToArray());
    }

    [TestMethod]
    public async Task SearchAsync_IncludingInactive_ReturnsInactiveItems()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.SearchAsync(new ProductSearchRequest("helmet", pageIndex: 0, pageSize: 10, includeInactive: true));

        Assert.AreEqual(1, result.TotalRecordCount);
        Assert.AreEqual("Helmet", result.Results.Single().Name);
    }

    [TestMethod]
    public async Task SearchAsync_NormalizesInvalidPagingValues()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.SearchAsync(new ProductSearchRequest(string.Empty, pageIndex: -4, pageSize: 0));

        Assert.AreEqual(0, result.PageIndex);
        Assert.AreEqual(10, result.PageSize);
        Assert.AreEqual(4, result.TotalRecordCount);
    }

    [TestMethod]
    public async Task SearchAsync_ComputesPagingNavigation()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.SearchAsync(new ProductSearchRequest(string.Empty, pageIndex: 1, pageSize: 2, includeInactive: true));

        Assert.AreEqual(5, result.TotalRecordCount);
        Assert.AreEqual(3, result.PageCount);
        Assert.AreEqual(0, result.PreviousPageIndex);
        Assert.AreEqual(2, result.NextPageIndex);
        Assert.AreEqual(2, result.PageRecordCount);
    }

    [TestMethod]
    public async Task FeaturedAsync_ReturnsHighestPricedActiveProducts()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.FeaturedAsync(2);

        CollectionAssert.AreEqual(new[] { "Hammer Drill", "Drill" }, result.Select(product => product.Name).ToArray());
    }

    [TestMethod]
    public async Task FeaturedAsync_UsesDefaultTake_WhenValueIsInvalid()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        var result = await service.FeaturedAsync(0);

        Assert.AreEqual(3, result.Count);
    }

    [TestMethod]
    public async Task SearchAsync_NullRequest_ThrowsArgumentNullException()
    {
        var service = new ProductCatalogService(new FakeRepository(BuildProducts()));

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.SearchAsync(null!));
    }
}
