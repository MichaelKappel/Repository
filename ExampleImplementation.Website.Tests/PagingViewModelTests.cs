using ExampleImplementation.Website.Models.DataModels;
using ExampleImplementation.Website.Models.ViewModels;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace ExampleImplementation.Website.Tests;

[TestClass]
public class PagingViewModelTests
{
    [TestMethod]
    public void PaggingControlViewModel_CopiesPagingAndRouteInformation()
    {
        var paging = new PagingResultsModel<ExampleDataViewModel>(
            totalRecordCount: 12,
            pageRecordCount: 5,
            pageCount: 3,
            previousPageIndex: 0,
            nextPageIndex: 2,
            pageIndex: 1,
            pageSize: 5,
            results: new List<ExampleDataViewModel>());

        var model = new PaggingControlViewModel(paging, "Home", "Index", routeId: 9, searchText: "tool");

        Assert.AreEqual("Home", model.Controller);
        Assert.AreEqual("Index", model.Action);
        Assert.AreEqual(9, model.RouteId);
        Assert.AreEqual("tool", model.SearchText);
        Assert.AreEqual(1, model.PageIndex);
        Assert.AreEqual(5, model.PageSize);
        Assert.AreEqual(12, model.TotalRecordCount);
        Assert.AreEqual(21, model.MaximumPageLinks);
    }

    [TestMethod]
    public void PaggingGridControlViewModel_CopiesPagingAndResults()
    {
        var data = new List<ExampleDataViewModel>
        {
            new() { Id = 1, Name = "A", Category = "Tools", Price = 10m },
            new() { Id = 2, Name = "B", Category = "Safety", Price = 20m }
        };
        var paging = new PagingResultsModel<ExampleDataViewModel>(new PagingModel(0, 2), 2, data);

        var model = new PaggingGridControlViewModel(paging, "Home", "Index", routeId: 0, results: data, searchText: "a");

        Assert.AreEqual(2, model.Results.Count);
        Assert.AreEqual("A", model.Results[0].Name);
        Assert.AreEqual("a", model.SearchText);
        Assert.AreEqual(0, model.PageIndex);
        Assert.AreEqual(2, model.PageSize);
        Assert.AreEqual(2, model.TotalRecordCount);
        Assert.AreEqual(21, model.MaximumPageLinks);
    }
}
