﻿using MichaelKappel.Repository.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryPagingAsync<T>
    {
        Task<IPagingResults<T>> List(IPaging page);
    }
    public interface IRepositoryPagingAsync<TResult, TKey>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey key);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4, TKey5>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7, TKey8 key8);
    }

    public interface IRepositoryPagingAsync<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8, TKey9>
    {
        Task<IPagingResults<TResult>> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7, TKey8 key8, TKey9 key9);
    }
}

