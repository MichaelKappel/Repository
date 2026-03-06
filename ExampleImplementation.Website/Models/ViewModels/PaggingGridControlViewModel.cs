using ExampleImplementation.Website.Models.DataModels;
using MichaelKappel.Repository.Interfaces.Models;

namespace ExampleImplementation.Website.Models.ViewModels
{
    public class PaggingGridControlViewModel : PaggingControlViewModel, IPagingResults<ExampleDataViewModel>
    {
        public PaggingGridControlViewModel(
            IPagingResults<ExampleDataViewModel> pagingDetail,
            string controller,
            string action,
            int routeId,
            IList<ExampleDataViewModel> results,
            string? searchText = null)
            : base(pagingDetail, controller, action, routeId, searchText)
        {
            Results = results;
        }

        public IList<ExampleDataViewModel> Results { get; set; }
    }
}
