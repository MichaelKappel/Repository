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
}