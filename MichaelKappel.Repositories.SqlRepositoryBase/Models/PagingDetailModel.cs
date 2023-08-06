using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Models
{
    public class PagingDetailModel : IPagingDetail
    {
        public PagingDetailModel(Int32 pageIndex, Int32 pageSize, Int32 totalRecordCount, Int32 pageRecordCount, Int32 pageCount, Int32? previousPageIndex, Int32? nextPageIndex)
        {
            TotalRecordCount = totalRecordCount;
            PageRecordCount = pageRecordCount;
            PageCount = pageCount;
            PreviousPageIndex = previousPageIndex;
            NextPageIndex = nextPageIndex;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagingDetailModel(IPaging paging, Int32 totalRecordCount)
        {
            Int32 reordsInPreviousPage = (paging.PageSize * paging.PageIndex);
            Int32 remainingRecordCount = totalRecordCount - reordsInPreviousPage;

            this.TotalRecordCount = totalRecordCount;
            this.PageRecordCount = (remainingRecordCount > paging.PageSize) ? paging.PageSize : totalRecordCount - reordsInPreviousPage;
            this.PageIndex = paging.PageIndex;
            this.PageSize = paging.PageSize;
            this.PageCount = (Int32)Math.Ceiling(TotalRecordCount / (decimal)PageSize);

            if (PageIndex > 0)
            {
                PreviousPageIndex = paging.PageIndex - 1;
            }
            if (PageIndex < PageCount - 1)
            {
                NextPageIndex = paging.PageIndex + 1;
            }
        }

        public Int32 TotalRecordCount { get; set; }

        public Int32 PageRecordCount { get; set; }

        public Int32 PageCount { get; set; }

        public Int32? PreviousPageIndex { get; set; }

        public Int32? NextPageIndex { get; set; }

        public Int32 PageIndex { get; set; }

        public Int32 PageSize { get; set; }
    }
}
