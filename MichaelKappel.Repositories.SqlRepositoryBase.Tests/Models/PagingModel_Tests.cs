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
    public class PagingModel_Tests
    {
        [TestMethod]
        public void PagingModel_Constructor_PageIndex_1_PageSize_43()
        {
            IPaging expected = new PagingModel(1, 43);


            IPaging actual = new PagingModel(expected);

            Assert.AreEqual(expected.PageIndex, actual.PageIndex);
            Assert.AreEqual(expected.PageSize, actual.PageSize);

        }
    }
}
