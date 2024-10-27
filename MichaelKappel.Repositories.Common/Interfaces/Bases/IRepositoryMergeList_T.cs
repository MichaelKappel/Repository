using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryMergeList<T, TUnsaved>
    {
        IList<T> MergeList(IList<TUnsaved> models);
    }

    public interface IRepositoryMergeList<TResult, TUnsaved, TKey2>
    {
        TResult MergeList(IList<TUnsaved> models, TKey2 key2);
    }

    public interface IRepositoryMergeList<TResult, TUnsaved, TKey2, TKey3>
    {
        TResult MergeList(IList<TUnsaved> models, TKey2 key2, TKey3 key3);
    }

    public interface IRepositoryMergeList<TResult, TUnsaved, TKey2, TKey3, TKey4>
    {
        TResult MergeList(IList<TUnsaved> models, TKey2 key2, TKey3 key3, TKey4 key4);
    }

}