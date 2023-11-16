using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryMergeAsync<T, TUnsaved>
    {
        Task<T> MergeAsync(TUnsaved model);
    }

    public interface IRepositoryMergeAsync<TResult, TKey1, TKey2>
    {
        Task<TResult> MergeAsync(TKey1 key1, TKey2 key2);
    }

    public interface IRepositoryMergeAsync<TResult, TKey1, TKey2, TKey3>
    {
        Task<TResult> MergeAsync(TKey1 key1, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryMergeAsync<TResult, TKey1, TKey2, TKey3, TKey4>
    {
        Task<TResult> MergeAsync(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
    }

}