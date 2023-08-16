using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Models
{
    public class PagingResultsModel<T> : IPagingResults<T> where T : class
    {

        public PagingResultsModel(IPaging paging, Int32 totalRecordCount, IList<T> results)
        {
            this.TotalRecordCount = totalRecordCount;
            this.PageRecordCount = results.Count();
            this.PageIndex = paging.PageIndex;
            this.PageSize = paging.PageSize;

            this.PageCount = (Int32)Math.Ceiling((Decimal)this.TotalRecordCount / (Decimal)this.PageSize);
            if (this.PageIndex > 0)
            {
                this.PreviousPageIndex = paging.PageIndex - 1;
            }
            if (this.PageIndex < (this.PageCount - 1))
            {
                this.NextPageIndex = paging.PageIndex + 1;
            }

            this.Results = results;
        }

        public PagingResultsModel(int totalRecordCount, int pageRecordCount, int pageCount, int? previousPageIndex, int? nextPageIndex, int pageIndex, int pageSize, IList<T> results)
        {
            this.TotalRecordCount = totalRecordCount;
            this.PageRecordCount = pageRecordCount;
            this.PageCount = pageCount;
            this.PreviousPageIndex = previousPageIndex;
            this.NextPageIndex = nextPageIndex;
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Results = results;
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
