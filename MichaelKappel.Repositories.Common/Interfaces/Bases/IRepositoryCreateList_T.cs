using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryCreateList<TResult, T>
    {
        IList<TResult> CreateList(IList<T> model);
    }

    public interface IRepositoryCreateList<TResult, TKey1, TKey2>
    {
        IList<TResult> CreateList(IList<TKey1> key1, TKey2 key2);
    }

}