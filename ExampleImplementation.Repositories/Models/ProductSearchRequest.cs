using MichaelKappel.Repositories.Common.Models;

namespace ExampleImplementation.Repositories.Models;

public sealed class ProductSearchRequest : PagingSearchByTextModel
{
    public ProductSearchRequest(string searchText, int pageIndex, int pageSize, bool includeInactive = false)
        : base(searchText, pageIndex, pageSize)
    {
        IncludeInactive = includeInactive;
    }

    public bool IncludeInactive { get; init; }
}
