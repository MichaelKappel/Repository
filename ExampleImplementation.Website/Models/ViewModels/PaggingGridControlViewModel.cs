using ExampleImplementation.Website.Models.DataModels;

using MichaelKappel.Repository.Interfaces.Models;

using Microsoft.AspNetCore.Mvc;

using System;

namespace ExampleImplementation.Website.Models.ViewModels
{
    public class PaggingGridControlViewModel : IPagingResults<ExampleDataViewModel>
    {

        public PaggingGridControlViewModel(IPagingResults<ExampleDataViewModel> pagingDetail, string controller, string action, int routeId, IList<ExampleDataViewModel> results)
        {
            this.Controller = controller;
            this.Action = action;
            this.RouteId = routeId;

            this.TotalRecordCount = pagingDetail.TotalRecordCount;
            this.PageRecordCount = pagingDetail.PageRecordCount;
            this.PageCount = pagingDetail.PageCount;
            this.PreviousPageIndex = pagingDetail.PreviousPageIndex;
            this.NextPageIndex = pagingDetail.NextPageIndex;
            this.PageIndex = pagingDetail.PageIndex;
            this.PageSize = pagingDetail.PageSize;
            this.Results = results;


            this.MaximumPageLinks = 21;
        }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int RouteId { get; set; }

        public int MaximumPageLinks { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }


        public int TotalRecordCount { get; set; }

        public int PageRecordCount { get; set; }

        public int PageCount { get; set; }


        public int? PreviousPageIndex { get; set; }

        public int? NextPageIndex { get; set; }

        public IList<ExampleDataViewModel> Results { get; set; }
    }
}
