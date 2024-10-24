using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryAll<T, TKey, TUnsaved> : 
        IRepository<T, TKey, TUnsaved>,
        IRepositoryAsync<T, TKey, TUnsaved>,
        IRepositoryPaging<T, TKey>,
        IRepositoryPagingAsync<T, TKey>
    {

    }
}