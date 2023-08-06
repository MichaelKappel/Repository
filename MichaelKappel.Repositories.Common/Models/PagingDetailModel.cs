using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.Common.Models
{
    public class PagingDetailModel : IPagingDetail
    {
        public PagingDetailModel(int pageIndex, int pageSize, int totalRecordCount, int pageRecordCount, int pageCount, int? previousPageIndex, int? nextPageIndex)
        {
            TotalRecordCount = totalRecordCount;
            PageRecordCount = pageRecordCount;
            PageCount = pageCount;
            PreviousPageIndex = previousPageIndex;
            NextPageIndex = nextPageIndex;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int TotalRecordCount { get; set; }

        public int PageRecordCount { get; set; }

        public int PageCount { get; set; }

        public int? PreviousPageIndex { get; set; }

        public int? NextPageIndex { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
