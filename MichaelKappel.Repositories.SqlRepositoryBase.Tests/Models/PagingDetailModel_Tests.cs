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
    public class PagingDetailModel_Tests
    {
        [TestMethod]
        public void PagingDetailModel_Constructor_PageIndex_1_PageSize_43_TotalRecords_530()
        {
            Int32 totalRecords = 530;
            IPaging paging = new PagingModel(1,43);
            PagingDetailModel expected = new(paging.PageIndex, paging.PageSize, totalRecords, 43, 13, 0, 2);


            PagingDetailModel actual = new(paging, totalRecords);

            Assert.AreEqual(expected.PageIndex, actual.PageIndex);
            Assert.AreEqual(expected.PageSize, actual.PageSize);
            Assert.AreEqual(expected.PageCount, actual.PageCount);

            Assert.AreEqual(expected.TotalRecordCount, actual.TotalRecordCount);
            Assert.AreEqual(expected.PageRecordCount, actual.PageRecordCount);

            Assert.AreEqual(expected.PreviousPageIndex, actual.PreviousPageIndex);
            Assert.AreEqual(expected.NextPageIndex, actual.NextPageIndex);

        }
    }
}
