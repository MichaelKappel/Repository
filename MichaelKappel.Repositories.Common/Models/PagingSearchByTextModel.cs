using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.Common.Models
{
    public class PagingSearchByTextModel : IPagingSearchByText
    {
        public PagingSearchByTextModel(string serachText, int pageIndex, int pageSize)
        {
            SearchText = serachText;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public PagingSearchByTextModel(string serachText)
        {
            SearchText = serachText;
        }

        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 100;

        public string SearchText { get; set; }
    }
}
