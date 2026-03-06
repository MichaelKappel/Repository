using System.Diagnostics;
using ExampleImplementation.Repositories.Abstractions;
using ExampleImplementation.Repositories.Models;
using ExampleImplementation.Website.Models;
using ExampleImplementation.Website.Models.DataModels;
using ExampleImplementation.Website.Models.ViewModels;
using MichaelKappel.Repositories.Common.Models;

using Microsoft.AspNetCore.Mvc;

namespace ExampleImplementation.Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductCatalogService _productCatalogService;

        public HomeController(ILogger<HomeController> logger, IProductCatalogService productCatalogService)
        {
            _logger = logger;
            _productCatalogService = productCatalogService;
        }

        public async Task<IActionResult> Index(string? searchText, int pageIndex = 0, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var request = new ProductSearchRequest(searchText ?? string.Empty, pageIndex, pageSize);
            var results = await _productCatalogService.SearchAsync(request, cancellationToken);

            var mappedResults = results.Results.Select(ExampleDataViewModel.FromModel).ToList();
            var pagingResults = new PagingResultsModel<ExampleDataViewModel>(
                results.TotalRecordCount,
                mappedResults.Count,
                results.PageCount,
                results.PreviousPageIndex,
                results.NextPageIndex,
                results.PageIndex,
                results.PageSize,
                mappedResults);

            var gridModel = new PaggingGridControlViewModel(
                pagingResults,
                controller: nameof(HomeController).Replace("Controller", string.Empty),
                action: nameof(Index),
                routeId: 0,
                results: mappedResults,
                searchText: searchText);

            var viewModel = new HomeIndexViewModel(searchText ?? string.Empty, gridModel);
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
