using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Models
{
    public class PagingModel : IPaging
    {
        public PagingModel()
        {
            this.PageIndex = 0;
            this.PageSize = 100;
        }

        public PagingModel(IPaging paging)
        {
            this.PageIndex = paging.PageIndex;
            this.PageSize = paging.PageSize;
        }

        public PagingModel(int pageIndex, int pageSize)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
