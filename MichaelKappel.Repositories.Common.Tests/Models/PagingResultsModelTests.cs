using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace MichaelKappel.Repositories.Common.Tests.Models;

[TestClass]
public class PagingResultsModelTests
{
    [TestMethod]
    public void Constructor_FromPaging_ComputesPagingState()
    {
        var paging = new PagingModel(1, 3);
        var results = new List<string> { "A", "B", "C" };

        var model = new PagingResultsModel<string>(paging, 10, results);

        Assert.AreEqual(10, model.TotalRecordCount);
        Assert.AreEqual(3, model.PageRecordCount);
        Assert.AreEqual(4, model.PageCount);
        Assert.AreEqual(0, model.PreviousPageIndex);
        Assert.AreEqual(2, model.NextPageIndex);
        CollectionAssert.AreEqual(results, model.Results.ToList());
    }

    [TestMethod]
    public void Constructor_FromPaging_ZeroRecords_HasNoNavigation()
    {
        var paging = new PagingModel(0, 10);

        var model = new PagingResultsModel<string>(paging, 0, new List<string>());

        Assert.AreEqual(0, model.PageCount);
        Assert.IsNull(model.PreviousPageIndex);
        Assert.IsNull(model.NextPageIndex);
        Assert.AreEqual(0, model.PageRecordCount);
    }

    [TestMethod]
    public void Constructor_WithAllValues_AssignsProperties()
    {
        var results = new List<string> { "X" };

        var model = new PagingResultsModel<string>(99, 1, 33, 10, 12, 11, 3, results);

        Assert.AreEqual(99, model.TotalRecordCount);
        Assert.AreEqual(1, model.PageRecordCount);
        Assert.AreEqual(33, model.PageCount);
        Assert.AreEqual(10, model.PreviousPageIndex);
        Assert.AreEqual(12, model.NextPageIndex);
        Assert.AreEqual(11, model.PageIndex);
        Assert.AreEqual(3, model.PageSize);
        CollectionAssert.AreEqual(results, model.Results.ToList());
    }
}
