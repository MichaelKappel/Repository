using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repository.Interfaces.Models
{
    public interface IPagingDetail : IPaging
    {
        int TotalRecordCount { get; }
        int PageRecordCount { get; }
        int PageCount { get; }
        int? PreviousPageIndex { get; }
        int? NextPageIndex { get; }
    }
}
