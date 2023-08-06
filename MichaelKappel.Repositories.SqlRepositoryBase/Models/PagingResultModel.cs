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

        public PagingResultModel(IPaging paging, int totalRecordCount, IList<T> results)
            :  base(paging,  totalRecordCount)
        {
            this.Results = results;
        }

        public IList<T> Results { get; protected set; }
    }
}
