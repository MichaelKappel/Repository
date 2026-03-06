using MichaelKappel.Repositories.SqlRepositoryBase.Models;

namespace MichaelKappel.Repositories.Common.Tests.Models;

[TestClass]
public class PagingModelTests
{
    [TestMethod]
    public void DefaultConstructor_UsesExpectedDefaults()
    {
        var model = new PagingModel();

        Assert.AreEqual(0, model.PageIndex);
        Assert.AreEqual(100, model.PageSize);
    }

    [TestMethod]
    public void CopyConstructor_CopiesValues()
    {
        var source = new PagingModel(3, 25);

        var model = new PagingModel(source);

        Assert.AreEqual(3, model.PageIndex);
        Assert.AreEqual(25, model.PageSize);
    }

    [TestMethod]
    public void ValueConstructor_AssignsValues()
    {
        var model = new PagingModel(4, 50);

        Assert.AreEqual(4, model.PageIndex);
        Assert.AreEqual(50, model.PageSize);
    }
}
