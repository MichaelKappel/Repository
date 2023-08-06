using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repository.Interfaces.Models
{
    public interface IPagingResults<T> : IPagingDetail
    {
        public IList<T> Results { get; }
    }
}
