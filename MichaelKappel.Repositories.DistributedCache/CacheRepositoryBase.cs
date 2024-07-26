
using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using MichaelKappel.Repositories.Common.JsonConverters;
using MichaelKappel.Repositories.Common.Models;

namespace MichaelKappel.Repositories.DistributedCache
{


    public class CacheRepositoryBase
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IDistributedCache _distributedCache;

        public CacheRepositoryBase(IDistributedCache distributedCache)
        {
            this._distributedCache = distributedCache;

            this._jsonSerializerOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
                PropertyNameCaseInsensitive = true
            };

            this._jsonSerializerOptions.Converters.Add(new LenientConverterBooleanNullable());
            this._jsonSerializerOptions.Converters.Add(new LenientConverterBoolean());
            this._jsonSerializerOptions.Converters.Add(new LenientConverterInt32Nullable());
            this._jsonSerializerOptions.Converters.Add(new LenientConverterInt32());
            this._jsonSerializerOptions.Converters.Add(new LenientConverterString());
            this._jsonSerializerOptions.Converters.Add(new LenientConverterDateOnly());
        }

        private async Task<T> GetFromCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFunction, TimeSpan? slidingExpiration = null) where T : class
        {
            byte[]? cachedData = await _distributedCache.GetAsync(cacheKey);
            if (cachedData != null)
            {
                //try
                //{
                JsonSerializerOptions jsoptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                T result = JsonSerializer.Deserialize<T>(cachedData, jsoptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize cached data.");
                }
                return result;
                //}
                //catch (JsonException ex)
                //{
                //    throw new  $"Deserialization failed for cache key {cacheKey}. Data: {Encoding.UTF8.GetString(cachedData)}");
                //    await _distributedCache.RemoveAsync(cacheKey);  // Clearing the problematic cache entry.
                //    return await fetchFunction(); // Fetching fresh data if cache fails.
                //}
            }

            T fromAlternative = await fetchFunction();
            byte[] dataToCache = JsonSerializer.SerializeToUtf8Bytes(fromAlternative, new JsonSerializerOptions { WriteIndented = true });
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromDays(30)
            };
            await _distributedCache.SetAsync(cacheKey, dataToCache, options);

            return fromAlternative;
        }

        private T GetFromCache<T>(string cacheKey, Func<T> fetchFunction, TimeSpan? slidingExpiration = null) where T : class
        {
            byte[]? cachedData = _distributedCache.Get(cacheKey);
            if (cachedData != null)
            {
                //try
                //{
                JsonSerializerOptions jsoptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                T result = JsonSerializer.Deserialize<T>(cachedData, jsoptions);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to deserialize cached data.");
                }
                return result;
                //}
                //catch (JsonException ex)
                //{
                //    _logger.LogError(ex, $"Json deserialization failed for cache key {cacheKey}. Data: {Encoding.UTF8.GetString(cachedData)}");
                //    _distributedCache.Remove(cacheKey);  // Clearing the problematic cache entry.
                //    return fetchFunction(); // Fetching fresh data if cache fails.
                //}
            }

            T fromAlternative = fetchFunction();
            byte[] dataToCache = JsonSerializer.SerializeToUtf8Bytes(fromAlternative, new JsonSerializerOptions { WriteIndented = true });
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = slidingExpiration ?? TimeSpan.FromHours(24)
            };
            _distributedCache.Set(cacheKey, dataToCache, options);

            return fromAlternative;
        }

        public Task<T> ReadCacheAsync<T>(string cacheKey, Func<Task<T>> fetchFunction) where T : class
        {
            return GetFromCacheAsync(cacheKey, fetchFunction);
        }

        public T ReadCache<T>(string cacheKey, Func<T> fetchFunction) where T : class
        {
            return GetFromCache(cacheKey, fetchFunction);
        }

        public Task<IList<T>> ListCacheAsync<T>(string cacheKey, Func<Task<IList<T>>> listFunction) where T : class
        {
            return GetFromCacheAsync<IList<T>>(cacheKey, listFunction);
        }

        public IList<T> ListCache<T>(string cacheKey, Func<IList<T>> listFunction) where T : class
        {
            return GetFromCache<IList<T>>(cacheKey, listFunction);
        }

        public Task<PagingResultsModel<T>> PagingCacheAsync<T>(string cacheKey, Func<Task<PagingResultsModel<T>>> pagingFunction) where T : class
        {
            return GetFromCacheAsync<PagingResultsModel<T>>(cacheKey, pagingFunction);
        }

        public PagingResultsModel<T> PagingCache<T>(string cacheKey, Func<PagingResultsModel<T>> pagingFunction) where T : class
        {
            return GetFromCache<PagingResultsModel<T>>(cacheKey, pagingFunction);
        }
    }
}
