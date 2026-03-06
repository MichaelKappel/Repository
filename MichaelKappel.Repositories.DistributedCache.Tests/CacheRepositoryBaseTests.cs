using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.DistributedCache;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using Moq;

namespace MichaelKappel.Repositories.DistributedCache.Tests;

[TestClass]
public class CacheRepositoryBaseTests
{
    private sealed class CacheItem
    {
        public string Name { get; set; } = string.Empty;
    }

    private static CacheRepositoryBase CreateRepository(Mock<IDistributedCache> distributedCache)
    {
        return new CacheRepositoryBase(distributedCache.Object);
    }

    [TestMethod]
    public void ReadCache_CacheHit_ReturnsCachedValue_WithoutCallingFallback()
    {
        var cacheKey = "cache-hit";
        var expected = new CacheItem { Name = "cached" };
        var payload = JsonSerializer.SerializeToUtf8Bytes(expected);
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.Get(cacheKey)).Returns(payload);

        var repository = CreateRepository(distributedCache);
        var fallbackCalled = false;

        var result = repository.ReadCache(cacheKey, () =>
        {
            fallbackCalled = true;
            return new CacheItem { Name = "fallback" };
        });

        Assert.AreEqual("cached", result.Name);
        Assert.IsFalse(fallbackCalled);
        distributedCache.Verify(cache => cache.Set(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()), Times.Never);
    }

    [TestMethod]
    public async Task ReadCacheAsync_CacheHit_ReturnsCachedValue_WithoutCallingFallback()
    {
        var cacheKey = "cache-hit-async";
        var expected = new CacheItem { Name = "cached" };
        var payload = JsonSerializer.SerializeToUtf8Bytes(expected);
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>())).ReturnsAsync(payload);

        var repository = CreateRepository(distributedCache);
        var fallbackCalled = false;

        var result = await repository.ReadCacheAsync(cacheKey, () =>
        {
            fallbackCalled = true;
            return Task.FromResult(new CacheItem { Name = "fallback" });
        });

        Assert.AreEqual("cached", result.Name);
        Assert.IsFalse(fallbackCalled);
        distributedCache.Verify(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public void ReadCache_CacheMiss_StoresUsingDefaultSlidingExpiration()
    {
        var cacheKey = "cache-miss";
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.Get(cacheKey)).Returns((byte[]?)null);

        DistributedCacheEntryOptions? capturedOptions = null;
        distributedCache
            .Setup(cache => cache.Set(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()))
            .Callback<string, byte[], DistributedCacheEntryOptions>((_, _, options) => capturedOptions = options);

        var repository = CreateRepository(distributedCache);

        var result = repository.ReadCache(cacheKey, () => new CacheItem { Name = "generated" });

        Assert.AreEqual("generated", result.Name);
        Assert.IsNotNull(capturedOptions);
        Assert.AreEqual(TimeSpan.FromHours(24), capturedOptions!.SlidingExpiration);
    }

    [TestMethod]
    public async Task ReadCacheAsync_CacheMiss_StoresUsingDefaultSlidingExpiration()
    {
        var cacheKey = "cache-miss-async";
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.GetAsync(cacheKey, It.IsAny<CancellationToken>())).ReturnsAsync((byte[]?)null);

        DistributedCacheEntryOptions? capturedOptions = null;
        distributedCache
            .Setup(cache => cache.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((_, _, options, _) => capturedOptions = options)
            .Returns(Task.CompletedTask);

        var repository = CreateRepository(distributedCache);

        var result = await repository.ReadCacheAsync(cacheKey, () => Task.FromResult(new CacheItem { Name = "generated" }));

        Assert.AreEqual("generated", result.Name);
        Assert.IsNotNull(capturedOptions);
        Assert.AreEqual(TimeSpan.FromDays(30), capturedOptions!.SlidingExpiration);
    }

    [TestMethod]
    public void ListCache_CacheHit_ReturnsCachedList()
    {
        var cacheKey = "list-cache-hit";
        var expected = new List<CacheItem>
        {
            new() { Name = "A" },
            new() { Name = "B" }
        };
        var payload = JsonSerializer.SerializeToUtf8Bytes<IList<CacheItem>>(expected);
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.Get(cacheKey)).Returns(payload);

        var repository = CreateRepository(distributedCache);

        var result = repository.ListCache<CacheItem>(cacheKey, () => throw new InvalidOperationException("fallback should not be called"));

        CollectionAssert.AreEqual(expected.Select(item => item.Name).ToList(), result.Select(item => item.Name).ToList());
    }

    [TestMethod]
    public void PagingCache_CacheMiss_StoresAndReturnsPagingModel()
    {
        var cacheKey = "paging-cache";
        var paging = new PagingModel(0, 2);
        var expected = new PagingResultsModel<string>(paging, 5, new List<string> { "X", "Y" });
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.Get(cacheKey)).Returns((byte[]?)null);

        byte[]? storedPayload = null;
        distributedCache
            .Setup(cache => cache.Set(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()))
            .Callback<string, byte[], DistributedCacheEntryOptions>((_, payload, _) => storedPayload = payload);

        var repository = CreateRepository(distributedCache);

        var result = repository.PagingCache(cacheKey, () => expected);

        Assert.AreEqual(expected.TotalRecordCount, result.TotalRecordCount);
        Assert.AreEqual(expected.PageRecordCount, result.PageRecordCount);
        CollectionAssert.AreEqual(expected.Results.ToList(), result.Results.ToList());
        Assert.IsNotNull(storedPayload);
    }

    [TestMethod]
    public void ReadCache_InvalidCachedData_ThrowsJsonException()
    {
        var distributedCache = new Mock<IDistributedCache>();
        distributedCache.Setup(cache => cache.Get("bad-json")).Returns(System.Text.Encoding.UTF8.GetBytes("not-json"));

        var repository = CreateRepository(distributedCache);

        Assert.ThrowsException<JsonException>(() => repository.ReadCache("bad-json", () => new CacheItem()));
    }
}
