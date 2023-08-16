using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using MichaelKappel.Repository.Interfaces.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests.Models
{
    [TestClass]
    public class PagingResultModel_Tests
    {

        [TestMethod]
        public void PagingResultModel_Constructor_PageIndex_1_PageSize_43_TotalRecords_530()
        {
            Int32 totalRecords = 530;
            IPaging paging = new PagingModel(1, 43);

            IList<String> pageItems = new List<String>();

            for (Int32 i =0; i < paging.PageSize; i++)
            {
                pageItems.Add($"Test {i}");
            }

            PagingResultsModel<String> expected = new(paging.PageIndex, paging.PageSize, totalRecords, 43, 13, 0, 2, pageItems);


            PagingResultsModel<String> actual = new(paging, totalRecords, pageItems);

            Assert.AreEqual(expected.PageIndex, actual.PageIndex);
            Assert.AreEqual(expected.PageSize, actual.PageSize);
            Assert.AreEqual(expected.PageCount, actual.PageCount);

            Assert.AreEqual(expected.TotalRecordCount, actual.TotalRecordCount);
            Assert.AreEqual(expected.PageRecordCount, actual.PageRecordCount);

            Assert.AreEqual(expected.PreviousPageIndex, actual.PreviousPageIndex);
            Assert.AreEqual(expected.NextPageIndex, actual.NextPageIndex);

            Assert.AreEqual(expected.Results.Count, actual.Results.Count);

        }
    }
}
