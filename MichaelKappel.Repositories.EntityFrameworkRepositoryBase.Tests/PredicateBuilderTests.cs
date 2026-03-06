using MichaelKappel.Repositories.EntityFrameworkRepositoryBase;
using System.Linq.Expressions;

namespace MichaelKappel.Repositories.EntityFrameworkRepositoryBase.Tests;

[TestClass]
public class PredicateBuilderTests
{
    private sealed class SampleEntity
    {
        public string Name { get; init; } = string.Empty;
        public int Score { get; init; }
    }

    [TestMethod]
    public void New_ReturnsEquivalentExpression()
    {
        Expression<Func<SampleEntity, bool>> expression = entity => entity.Score > 10;

        var predicate = PredicateBuilder.New(expression).Compile();

        Assert.IsTrue(predicate(new SampleEntity { Score = 11 }));
        Assert.IsFalse(predicate(new SampleEntity { Score = 10 }));
    }

    [TestMethod]
    public void And_CombinesDifferentParameterExpressions()
    {
        Expression<Func<SampleEntity, bool>> hasAName = first => first.Name.StartsWith("A");
        Expression<Func<SampleEntity, bool>> highScore = second => second.Score >= 90;

        var combined = hasAName.And(highScore);
        var predicate = combined.Compile();

        Assert.AreEqual(1, combined.Parameters.Count);
        Assert.IsTrue(predicate(new SampleEntity { Name = "Alice", Score = 95 }));
        Assert.IsFalse(predicate(new SampleEntity { Name = "Alice", Score = 70 }));
        Assert.IsFalse(predicate(new SampleEntity { Name = "Bob", Score = 95 }));
    }

    [TestMethod]
    public void Or_CombinesDifferentParameterExpressions()
    {
        Expression<Func<SampleEntity, bool>> hasAName = first => first.Name.StartsWith("A");
        Expression<Func<SampleEntity, bool>> highScore = second => second.Score >= 90;

        var combined = hasAName.Or(highScore).Compile();

        Assert.IsTrue(combined(new SampleEntity { Name = "Alice", Score = 70 }));
        Assert.IsTrue(combined(new SampleEntity { Name = "Bob", Score = 95 }));
        Assert.IsFalse(combined(new SampleEntity { Name = "Bob", Score = 70 }));
    }
}
