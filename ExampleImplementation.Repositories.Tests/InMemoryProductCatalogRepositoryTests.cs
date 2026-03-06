using ExampleImplementation.Repositories.Repositories;

namespace ExampleImplementation.Repositories.Tests;

[TestClass]
public class InMemoryProductCatalogRepositoryTests
{
    [TestMethod]
    public async Task ListAsync_ReturnsSeedData()
    {
        var repository = new InMemoryProductCatalogRepository();

        var items = await repository.ListAsync();

        Assert.IsTrue(items.Count >= 10);
        Assert.IsTrue(items.Any(item => item.IsActive));
    }

    [TestMethod]
    public async Task ListAsync_ReturnsIndependentCopies()
    {
        var repository = new InMemoryProductCatalogRepository();

        var first = await repository.ListAsync();
        var second = await repository.ListAsync();

        Assert.AreNotSame(first, second);
    }
}
