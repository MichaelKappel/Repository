using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryMerge<T, TUnsaved>
    {
        T Merge(TUnsaved model);
    }

    public interface IRepositoryMerge<TResult, TKey1, TKey2>
    {
        TResult Merge(TKey1 key1, TKey2 key2);
    }

    public interface IRepositoryMerge<TResult, TKey1, TKey2, TKey3>
    {
        TResult Merge(TKey1 key1, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryMerge<TResult, TKey1, TKey2, TKey3, TKey4>
    {
        TResult Merge(TKey1 key1, TKey2 key2, TKey3 key3, TKey4 key4);
    }

}