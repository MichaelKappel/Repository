using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositorySearchAsync<TResult, IPagingKey>
       where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> SearchAsync(IPagingKey pagingKey);
    }

    public interface IRepositorySearchAsync<TResult, IPagingKey, TKey>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> SearchAsync(IPagingKey pagingKey, TKey key);
    }

    public interface IRepositorySearchAsync<TResult, IPagingKey, TKey1, TKey2>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> SearchAsync(IPagingKey pagingKey, TKey1 key1, TKey2 key2);
    }

    public interface IRepositorySearchAsync<TResult, IPagingKey, TKey1, TKey2, TKey3>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> SearchAsync(IPagingKey pagingKey, TKey1 key1, TKey2 key2, TKey3 key3);
    }
}