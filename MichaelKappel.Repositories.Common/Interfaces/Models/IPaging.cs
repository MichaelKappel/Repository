using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repository.Interfaces.Models
{
    public interface IPaging
    {
        public int PageIndex { get; }
        public int PageSize { get; }
    }
}
