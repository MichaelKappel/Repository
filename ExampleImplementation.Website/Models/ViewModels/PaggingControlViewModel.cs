using MichaelKappel.Repository.Interfaces.Models;

namespace ExampleImplementation.Website.Models.ViewModels
{
    public class PaggingControlViewModel : IPagingDetail
    {
        public PaggingControlViewModel(IPagingDetail pagingDetail, string controller, string action, int routeId, string? searchText = null)
        {
            Controller = controller;
            Action = action;
            RouteId = routeId;
            SearchText = searchText;

            TotalRecordCount = pagingDetail.TotalRecordCount;
            PageRecordCount = pagingDetail.PageRecordCount;
            PageCount = pagingDetail.PageCount;
            PreviousPageIndex = pagingDetail.PreviousPageIndex;
            NextPageIndex = pagingDetail.NextPageIndex;
            PageIndex = pagingDetail.PageIndex;
            PageSize = pagingDetail.PageSize;

            MaximumPageLinks = 21;
        }

        public string Controller { get; set; }
        public string Action { get; set; }
        public int RouteId { get; set; }
        public int MaximumPageLinks { get; set; }
        public string? SearchText { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
        public int PageRecordCount { get; set; }
        public int PageCount { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
    }
}
