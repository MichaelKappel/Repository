using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryMergeListAsync<T, TUnsaved>
    {
        Task<IList<T>> MergeListAsync(IList<TUnsaved> models);
    }

    public interface IRepositoryMergeListAsync<TResult, TUnsaved, TKey2>
    {
        Task<IList<TResult>> MergeListAsync(IList<TUnsaved> models, TKey2 key2);
    }

    public interface IRepositoryMergeListAsync<TResult, TUnsaved, TKey2, TKey3>
    {
        Task<IList<TResult>> MergeListAsync(IList<TUnsaved> models, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryMergeListAsync<TResult, TUnsaved, TKey2, TKey3, TKey4>
    {
        Task<IList<TResult>> MergeListAsync(IList<TUnsaved> models, TKey2 key2, TKey3 key3, TKey4 key4);
    }

}