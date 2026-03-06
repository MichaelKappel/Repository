using MichaelKappel.Repositories.Common.Models;

namespace MichaelKappel.Repositories.Common.Tests.Models;

[TestClass]
public class PagingSearchByTextModelTests
{
    [TestMethod]
    public void Constructor_WithSearchOnly_UsesDefaults()
    {
        var model = new PagingSearchByTextModel("demo");

        Assert.AreEqual("demo", model.SearchText);
        Assert.AreEqual(1, model.PageIndex);
        Assert.AreEqual(100, model.PageSize);
    }

    [TestMethod]
    public void Constructor_WithAllValues_AssignsProperties()
    {
        var model = new PagingSearchByTextModel("term", 2, 50);

        Assert.AreEqual("term", model.SearchText);
        Assert.AreEqual(2, model.PageIndex);
        Assert.AreEqual(50, model.PageSize);
    }
}
