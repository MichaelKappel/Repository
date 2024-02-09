using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryCreateListAsync<TResult, T>
    {
        Task<IList<TResult>> CreateListAsync(IList<T> model);
    }

    public interface IRepositoryCreateListAsync<TResult, TKey1, TKey2>
    {
        Task<IList<TResult>> CreateListAsync(IList<TKey1> key1, TKey2 key2);
    }
}