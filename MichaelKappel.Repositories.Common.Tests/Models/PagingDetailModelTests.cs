using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace MichaelKappel.Repositories.Common.Tests.Models;

[TestClass]
public class PagingDetailModelTests
{
    [TestMethod]
    public void Constructor_FromPaging_FirstPage_ComputesNavigation()
    {
        var paging = new PagingModel(0, 10);

        var model = new PagingDetailModel(paging, 26);

        Assert.AreEqual(26, model.TotalRecordCount);
        Assert.AreEqual(10, model.PageRecordCount);
        Assert.AreEqual(3, model.PageCount);
        Assert.IsNull(model.PreviousPageIndex);
        Assert.AreEqual(1, model.NextPageIndex);
    }

    [TestMethod]
    public void Constructor_FromPaging_LastPage_ComputesNavigation()
    {
        var paging = new PagingModel(2, 10);

        var model = new PagingDetailModel(paging, 26);

        Assert.AreEqual(6, model.PageRecordCount);
        Assert.AreEqual(3, model.PageCount);
        Assert.AreEqual(1, model.PreviousPageIndex);
        Assert.IsNull(model.NextPageIndex);
    }

    [TestMethod]
    public void Constructor_WithAllValues_AssignsProperties()
    {
        var model = new PagingDetailModel(1, 20, 100, 20, 5, 0, 2);

        Assert.AreEqual(100, model.TotalRecordCount);
        Assert.AreEqual(20, model.PageRecordCount);
        Assert.AreEqual(5, model.PageCount);
        Assert.AreEqual(0, model.PreviousPageIndex);
        Assert.AreEqual(2, model.NextPageIndex);
        Assert.AreEqual(1, model.PageIndex);
        Assert.AreEqual(20, model.PageSize);
    }
}
