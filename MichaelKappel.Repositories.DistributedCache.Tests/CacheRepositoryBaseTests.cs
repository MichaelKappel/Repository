using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MichaelKappel.Repositories.DistributedCache;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace MichaelKappel.Repositories.DistributedCache.Tests;

[TestClass]
public class CacheRepositoryBaseTests
{
    private CacheRepositoryBase CreateRepo(Mock<IDistributedCache> mockCache)
    {
        return new CacheRepositoryBase(mockCache.Object);
    }

    [TestMethod]
    public async Task ReadCacheAsync_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "key";
        var expected = "value";
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((k, b, o, t) => stored = b)
                 .Returns(Task.CompletedTask);

        var repo = CreateRepo(mockCache);
        var result = await repo.ReadCacheAsync(cacheKey, () => Task.FromResult(expected));

        Assert.AreEqual(expected, result);
        Assert.IsNotNull(stored);
    }

    [TestMethod]
    public void ReadCache_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "key";
        var expected = "value";
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.Get(cacheKey)).Returns((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.Set(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions>((k, b, o) => stored = b);

        var repo = CreateRepo(mockCache);
        var result = repo.ReadCache(cacheKey, () => expected);

        Assert.AreEqual(expected, result);
        Assert.IsNotNull(stored);
    }

    [TestMethod]
    public async Task ListCacheAsync_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "list";
        var expected = new List<string> { "a", "b" } as IList<string>;
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((k, b, o, t) => stored = b)
                 .Returns(Task.CompletedTask);

        var repo = CreateRepo(mockCache);
        var result = await repo.ListCacheAsync(cacheKey, () => Task.FromResult(expected));

        CollectionAssert.AreEqual((List<string>)expected, (List<string>)result);
        Assert.IsNotNull(stored);
    }

    [TestMethod]
    public void ListCache_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "list";
        var expected = new List<string> { "a", "b" } as IList<string>;
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.Get(cacheKey)).Returns((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.Set(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions>((k, b, o) => stored = b);

        var repo = CreateRepo(mockCache);
        var result = repo.ListCache(cacheKey, () => expected);

        CollectionAssert.AreEqual((List<string>)expected, (List<string>)result);
        Assert.IsNotNull(stored);
    }

    [TestMethod]
    public async Task PagingCacheAsync_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "paging";
        var paging = new PagingModel(0, 10);
        var expected = new PagingResultsModel<string>(paging, 2, new List<string> { "x", "y" });
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions, CancellationToken>((k, b, o, t) => stored = b)
                 .Returns(Task.CompletedTask);

        var repo = CreateRepo(mockCache);
        var result = await repo.PagingCacheAsync(cacheKey, () => Task.FromResult(expected));

        Assert.AreEqual(expected.TotalRecordCount, result.TotalRecordCount);
        CollectionAssert.AreEqual((List<string>)expected.Results, (List<string>)result.Results);
        Assert.IsNotNull(stored);
    }

    [TestMethod]
    public void PagingCache_CacheMiss_StoresAndReturns()
    {
        var cacheKey = "paging";
        var paging = new PagingModel(0, 10);
        var expected = new PagingResultsModel<string>(paging, 2, new List<string> { "x", "y" });
        var mockCache = new Mock<IDistributedCache>();
        mockCache.Setup(c => c.Get(cacheKey)).Returns((byte[]?)null);
        byte[]? stored = null;
        mockCache.Setup(c => c.Set(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()))
                 .Callback<string, byte[], DistributedCacheEntryOptions>((k, b, o) => stored = b);

        var repo = CreateRepo(mockCache);
        var result = repo.PagingCache(cacheKey, () => expected);

        Assert.AreEqual(expected.TotalRecordCount, result.TotalRecordCount);
        CollectionAssert.AreEqual((List<string>)expected.Results, (List<string>)result.Results);
        Assert.IsNotNull(stored);
    }
}
