using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repository.Interfaces.Models;
using System.Collections.Generic;
using System.Linq;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests
{
    [TestClass]
    public class PagingResultsModel_Tests
    {
        private PagingResultsModel<string> CreateModel(int pageIndex, int pageSize, int totalRecords)
        {
            var mockPaging = new Mock<IPaging>();
            mockPaging.SetupGet(p => p.PageIndex).Returns(pageIndex);
            mockPaging.SetupGet(p => p.PageSize).Returns(pageSize);
            var results = Enumerable.Range(0, pageSize).Select(i => $"Item{i}").ToList();
            return new PagingResultsModel<string>(mockPaging.Object, totalRecords, results);
        }

        // Generate 50 tests with different page index and size combinations
        private void VerifyModel(int pageIndex, int pageSize, int totalRecords)
        {
            var model = CreateModel(pageIndex, pageSize, totalRecords);
            var expectedPageCount = (int)System.Math.Ceiling((decimal)totalRecords / pageSize);
            Assert.AreEqual(expectedPageCount, model.PageCount);
            Assert.AreEqual(pageIndex, model.PageIndex);
            Assert.AreEqual(pageSize, model.PageSize);
            var expectedPrev = pageIndex > 0 ? pageIndex - 1 : (int?)null;
            var expectedNext = pageIndex < expectedPageCount - 1 ? pageIndex + 1 : (int?)null;
            Assert.AreEqual(expectedPrev, model.PreviousPageIndex);
            Assert.AreEqual(expectedNext, model.NextPageIndex);
        }

        [TestMethod]
        public void PagingResult_0()
        {
            VerifyModel(0, 10, 100);
        }

        [TestMethod]
        public void PagingResult_1()
        {
            VerifyModel(1, 20, 100);
        }

        [TestMethod]
        public void PagingResult_2()
        {
            VerifyModel(2, 30, 100);
        }

        [TestMethod]
        public void PagingResult_3()
        {
            VerifyModel(3, 10, 100);
        }

        [TestMethod]
        public void PagingResult_4()
        {
            VerifyModel(4, 20, 100);
        }

        [TestMethod]
        public void PagingResult_5()
        {
            VerifyModel(0, 30, 100);
        }

        [TestMethod]
        public void PagingResult_6()
        {
            VerifyModel(1, 10, 100);
        }

        [TestMethod]
        public void PagingResult_7()
        {
            VerifyModel(2, 20, 100);
        }

        [TestMethod]
        public void PagingResult_8()
        {
            VerifyModel(3, 30, 100);
        }

        [TestMethod]
        public void PagingResult_9()
        {
            VerifyModel(4, 10, 100);
        }

        [TestMethod]
        public void PagingResult_10()
        {
            VerifyModel(0, 20, 100);
        }

        [TestMethod]
        public void PagingResult_11()
        {
            VerifyModel(1, 30, 100);
        }

        [TestMethod]
        public void PagingResult_12()
        {
            VerifyModel(2, 10, 100);
        }

        [TestMethod]
        public void PagingResult_13()
        {
            VerifyModel(3, 20, 100);
        }

        [TestMethod]
        public void PagingResult_14()
        {
            VerifyModel(4, 30, 100);
        }

        [TestMethod]
        public void PagingResult_15()
        {
            VerifyModel(0, 10, 100);
        }

        [TestMethod]
        public void PagingResult_16()
        {
            VerifyModel(1, 20, 100);
        }

        [TestMethod]
        public void PagingResult_17()
        {
            VerifyModel(2, 30, 100);
        }

        [TestMethod]
        public void PagingResult_18()
        {
            VerifyModel(3, 10, 100);
        }

        [TestMethod]
        public void PagingResult_19()
        {
            VerifyModel(4, 20, 100);
        }

        [TestMethod]
        public void PagingResult_20()
        {
            VerifyModel(0, 30, 100);
        }

        [TestMethod]
        public void PagingResult_21()
        {
            VerifyModel(1, 10, 100);
        }

        [TestMethod]
        public void PagingResult_22()
        {
            VerifyModel(2, 20, 100);
        }

        [TestMethod]
        public void PagingResult_23()
        {
            VerifyModel(3, 30, 100);
        }

        [TestMethod]
        public void PagingResult_24()
        {
            VerifyModel(4, 10, 100);
        }

        [TestMethod]
        public void PagingResult_25()
        {
            VerifyModel(0, 20, 100);
        }

        [TestMethod]
        public void PagingResult_26()
        {
            VerifyModel(1, 30, 100);
        }

        [TestMethod]
        public void PagingResult_27()
        {
            VerifyModel(2, 10, 100);
        }

        [TestMethod]
        public void PagingResult_28()
        {
            VerifyModel(3, 20, 100);
        }

        [TestMethod]
        public void PagingResult_29()
        {
            VerifyModel(4, 30, 100);
        }

        [TestMethod]
        public void PagingResult_30()
        {
            VerifyModel(0, 10, 100);
        }

        [TestMethod]
        public void PagingResult_31()
        {
            VerifyModel(1, 20, 100);
        }

        [TestMethod]
        public void PagingResult_32()
        {
            VerifyModel(2, 30, 100);
        }

        [TestMethod]
        public void PagingResult_33()
        {
            VerifyModel(3, 10, 100);
        }

        [TestMethod]
        public void PagingResult_34()
        {
            VerifyModel(4, 20, 100);
        }

        [TestMethod]
        public void PagingResult_35()
        {
            VerifyModel(0, 30, 100);
        }

        [TestMethod]
        public void PagingResult_36()
        {
            VerifyModel(1, 10, 100);
        }

        [TestMethod]
        public void PagingResult_37()
        {
            VerifyModel(2, 20, 100);
        }

        [TestMethod]
        public void PagingResult_38()
        {
            VerifyModel(3, 30, 100);
        }

        [TestMethod]
        public void PagingResult_39()
        {
            VerifyModel(4, 10, 100);
        }

        [TestMethod]
        public void PagingResult_40()
        {
            VerifyModel(0, 20, 100);
        }

        [TestMethod]
        public void PagingResult_41()
        {
            VerifyModel(1, 30, 100);
        }

        [TestMethod]
        public void PagingResult_42()
        {
            VerifyModel(2, 10, 100);
        }

        [TestMethod]
        public void PagingResult_43()
        {
            VerifyModel(3, 20, 100);
        }

        [TestMethod]
        public void PagingResult_44()
        {
            VerifyModel(4, 30, 100);
        }

        [TestMethod]
        public void PagingResult_45()
        {
            VerifyModel(0, 10, 100);
        }

        [TestMethod]
        public void PagingResult_46()
        {
            VerifyModel(1, 20, 100);
        }

        [TestMethod]
        public void PagingResult_47()
        {
            VerifyModel(2, 30, 100);
        }

        [TestMethod]
        public void PagingResult_48()
        {
            VerifyModel(3, 10, 100);
        }

        [TestMethod]
        public void PagingResult_49()
        {
            VerifyModel(4, 20, 100);
        }

    }\n}
