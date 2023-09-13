using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositorySearch<TResult, IPagingKey>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> Search(IPagingKey pagingKey);
    }

    public interface IRepositorySearch<TResult, IPagingKey, TKey>
        where IPagingKey : IPagingSearch
    {
         IPagingResults<TResult> Search(IPagingKey pagingKey, TKey key);
    }

    public interface IRepositorySearch<TResult, IPagingKey, TKey1, TKey2>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> Search(IPagingKey pagingKey, TKey1 key1, TKey2 key2);
    }

    public interface IRepositorySearch<TResult, IPagingKey, TKey1, TKey2, TKey3>
        where IPagingKey : IPagingSearch
    {
        IPagingResults<TResult> Search(IPagingKey pagingKey, TKey1 key1, TKey2 key2, TKey3 key3);
    }
}