using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MichaelKappel.Repository.Interfaces.Bases
{
    public interface IRepositoryMergeAsync<T, TUnsaved>
    {
        Task<T> Merge(TUnsaved model);
    }
}