using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepository<T, TKey> : IRepositoryCreate<T>, IRepositoryRead<T, TKey>, IRepositoryUpdate<T>, IRepositoryDelete<T, TKey>, IRepositoryList<T>
    {

    }

}