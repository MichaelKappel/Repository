namespace ExampleImplementation.Website.Models.ViewModels
{
    public class HomeIndexViewModel
    {
        public HomeIndexViewModel(string searchText, PaggingGridControlViewModel products)
        {
            SearchText = searchText;
            Products = products;
        }

        public string SearchText { get; }
        public PaggingGridControlViewModel Products { get; }
    }
}
