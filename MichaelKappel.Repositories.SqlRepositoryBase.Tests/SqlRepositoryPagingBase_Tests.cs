using System.Data;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Tests.TestDoubles;
using Microsoft.Data.SqlClient;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests;

[TestClass]
public class SqlRepositoryPagingBaseTests
{
    [TestMethod]
    public void AddPaging_TextCommand_AppendsPagingSqlAndParameters()
    {
        var repository = new TestPagingRepository();
        var paging = new PagingModel(2, 25);
        var commandType = CommandType.Text;
        var sql = "SELECT * FROM Items ORDER BY Id";
        SqlParameter[] parameters =
        {
            new("@Type", SqlDbType.VarChar) { Value = "A" }
        };

        repository.ExposedAddPaging(paging, ref commandType, ref sql, ref parameters);

        StringAssert.Contains(sql, "OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY");
        Assert.AreEqual(3, parameters.Length);
        Assert.AreEqual("@PageIndex", parameters[1].ParameterName);
        Assert.AreEqual(2, parameters[1].Value);
        Assert.AreEqual("@PageSize", parameters[2].ParameterName);
        Assert.AreEqual(25, parameters[2].Value);
    }

    [TestMethod]
    public void AddPaging_NonTextCommand_Throws()
    {
        var repository = new TestPagingRepository();
        var paging = new PagingModel(0, 10);
        var commandType = CommandType.StoredProcedure;
        var sql = "dbo.GetItems";
        var parameters = Array.Empty<SqlParameter>();

        var ex = Assert.ThrowsException<Exception>(() => repository.ExposedAddPaging(paging, ref commandType, ref sql, ref parameters));

        StringAssert.Contains(ex.Message, "only supports commandType of Text");
    }

    [TestMethod]
    public void GetPagingResults_BuildsCountAndPagingQueries()
    {
        var repository = new TestPagingRepository
        {
            CountToReturn = 5,
            ModelsToReturn = new List<TestSqlRepository.TestRow>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }
        };
        var paging = new PagingModel(1, 2);
        var sql = $"SELECT Id FROM dbo.Items WHERE Category = @Category ORDER BY Id --{Guid.NewGuid():N}";
        var parameters = new[]
        {
            new SqlParameter("@Category", SqlDbType.VarChar) { Value = "Tools" }
        };

        var result = repository.ExposedGetPagingResults(paging, sql, CommandType.Text, parameters);

        Assert.AreEqual(5, result.TotalRecordCount);
        Assert.AreEqual(3, result.PageCount);
        Assert.AreEqual(0, result.PreviousPageIndex);
        Assert.AreEqual(2, result.NextPageIndex);
        Assert.AreEqual(2, result.Results.Count);

        Assert.IsNotNull(repository.LastCountSql);
        StringAssert.Contains(repository.LastCountSql, "SELECT COUNT(cteRecordCount.FakeColumn) FROM");
        Assert.IsFalse(repository.LastCountSql.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase));

        Assert.IsNotNull(repository.LastModelsSql);
        StringAssert.Contains(repository.LastModelsSql, "OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY");
        Assert.AreEqual(CommandType.Text, repository.LastModelsCommandType);
        Assert.AreEqual(3, repository.LastModelsParameters.Length);
    }

    [TestMethod]
    public async Task GetPagingResultsAsync_BuildsCountAndPagingQueries()
    {
        var repository = new TestPagingRepository
        {
            CountToReturn = 4,
            ModelsToReturn = new List<TestSqlRepository.TestRow>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }
        };
        var paging = new PagingModel(0, 2);
        var sql = $"SELECT Id FROM dbo.Items ORDER BY Id --{Guid.NewGuid():N}";

        var result = await repository.ExposedGetPagingResultsAsync(paging, sql, CommandType.Text);

        Assert.AreEqual(4, result.TotalRecordCount);
        Assert.AreEqual(2, result.PageCount);
        Assert.AreEqual(2, result.Results.Count);
        Assert.AreEqual(1, repository.ExecuteScalarCallCount);
        StringAssert.Contains(repository.LastModelsSql!, "OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY");
    }

    [TestMethod]
    public void GetPagingResults_CachesRecordCount_ForIdenticalQueries()
    {
        var repository = new TestPagingRepository
        {
            CountToReturn = 10,
            ModelsToReturn = new List<TestSqlRepository.TestRow> { new() { Id = 1 } }
        };
        var paging = new PagingModel(0, 1);
        var sql = $"SELECT Id FROM dbo.Items ORDER BY Id --{Guid.NewGuid():N}";

        repository.ExposedGetPagingResults(paging, sql, CommandType.Text);
        repository.ExposedGetPagingResults(paging, sql, CommandType.Text);

        Assert.AreEqual(1, repository.ExecuteScalarCallCount);
    }
}
