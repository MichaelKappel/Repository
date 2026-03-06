using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;
using ExampleImplementation.Website.Controllers;
using ExampleImplementation.Website.Models;
using ExampleImplementation.Website.Models.ViewModels;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ExampleImplementation.Website.Tests;

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public async Task Index_ReturnsMappedViewModel()
    {
        var service = new Mock<IProductCatalogService>();
        var paging = new PagingModel(1, 2);
        var sourceResults = new List<ExampleProduct>
        {
            new() { Id = 10, Name = "Laser Level", Category = "Measuring", Price = 119.95m, IsActive = true },
            new() { Id = 11, Name = "Wrench Set", Category = "Tools", Price = 49.99m, IsActive = true }
        };
        var pagedResults = new PagingResultsModel<ExampleProduct>(paging, totalRecordCount: 5, sourceResults);

        service
            .Setup(catalog => catalog.SearchAsync(It.IsAny<ProductSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pagedResults);

        var logger = new Mock<ILogger<HomeController>>();
        var controller = new HomeController(logger.Object, service.Object);

        var actionResult = await controller.Index("tool", pageIndex: 1, pageSize: 2);

        var viewResult = actionResult as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as HomeIndexViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("tool", model.SearchText);
        Assert.AreEqual(2, model.Products.Results.Count);
        Assert.AreEqual("Laser Level", model.Products.Results[0].Name);
        Assert.AreEqual(5, model.Products.TotalRecordCount);

        service.Verify(catalog => catalog.SearchAsync(
            It.Is<ProductSearchRequest>(request => request.SearchText == "tool" && request.PageIndex == 1 && request.PageSize == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public void Error_ReturnsErrorViewModel_WithTraceIdentifier()
    {
        var service = new Mock<IProductCatalogService>();
        var logger = new Mock<ILogger<HomeController>>();
        var controller = new HomeController(logger.Object, service.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    TraceIdentifier = "trace-123"
                }
            }
        };

        var actionResult = controller.Error();

        var viewResult = actionResult as ViewResult;
        Assert.IsNotNull(viewResult);

        var model = viewResult.Model as ErrorViewModel;
        Assert.IsNotNull(model);
        Assert.AreEqual("trace-123", model.RequestId);
        Assert.IsTrue(model.ShowRequestId);
    }
}
