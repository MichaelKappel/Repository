using MichaelKappel.Repository.Interfaces.Models;
using System;
using System.Collections.Generic;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryPaging<T>
    {
        IPagingResults<T> List(IPaging page);
    }
    public interface IRepositoryPaging<TResult, TKey>
    {
        IPagingResults<TResult> List(IPaging page, TKey key);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4, TKey5>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7, TKey8 key8);
    }

    public interface IRepositoryPaging<TResult, TKey1, TKey2, TKey3, TKey4, TKey5, TKey6, TKey7, TKey8, TKey9>
    {
        IPagingResults<TResult> List(IPaging page, TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4, TKey5 key5, TKey6 key6, TKey7 key7, TKey8 key8, TKey9 key9);
    }
}

