using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Models
{
    public class PagingResultModel<T> : PagingDetailModel, IPagingResults<T> where T : class
    {

        public PagingResultModel(Int32 pageIndex, Int32 pageSize, Int32 totalRecordCount, Int32 pageRecordCount, Int32 pageCount, Int32? previousPageIndex, Int32? nextPageIndex, IList<T> results)
            : base(pageIndex, pageSize, totalRecordCount,  pageRecordCount,  pageCount, previousPageIndex, nextPageIndex)
        {
            this.Results = results;
        }

        public PagingResultModel(IPaging paging, int totalRecordCount, IList<T> results)
            :  base(paging,  totalRecordCount)
        {
            this.Results = results;
        }

        public IList<T> Results { get; protected set; }
    }
}
