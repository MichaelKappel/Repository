using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.Common.Models
{
    public class PagingResultModel<T> : IPagingResults<T> where T : class
    {

        public PagingResultModel(IPaging paging, int totalRecordCount, IList<T> results)
        {
            TotalRecordCount = totalRecordCount;
            PageRecordCount = results.Count();
            PageIndex = paging.PageIndex;
            PageSize = paging.PageSize;

            PageCount = (int)Math.Floor(TotalRecordCount / (decimal)PageSize);
            if (PageIndex > 0)
            {
                PreviousPageIndex = paging.PageIndex - 1;
            }
            if (PageIndex < PageCount - 1)
            {
                NextPageIndex = paging.PageIndex + 1;
            }

            Results = results;
        }

        public int TotalRecordCount { get; protected set; }

        public int PageRecordCount { get; protected set; }

        public int PageCount { get; protected set; }

        public int? PreviousPageIndex { get; protected set; }

        public int? NextPageIndex { get; protected set; }

        public int PageIndex { get; protected set; }

        public int PageSize { get; protected set; }

        public IList<T> Results { get; protected set; }
    }
}
