using System.Data;
using MichaelKappel.Repositories.SqlRepositoryBase.Tests.TestDoubles;
using Microsoft.Data.SqlClient;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests;

[TestClass]
public class GetFullSqlTests
{
    [TestMethod]
    public void StoredProcedure_AddsExecAndParameters()
    {
        var repository = new TestSqlRepository();
        var parameter = new SqlParameter("@Id", SqlDbType.Int) { Value = 5 };

        var sql = repository.ExposedGetFullSql("dbo.MyProc", CommandType.StoredProcedure, parameter).Trim();

        Assert.AreEqual("DECLARE @Id AS Int = 5;\r\nEXEC dbo.MyProc @Id = @Id", sql);
    }

    [TestMethod]
    public void StoredProcedure_WhenSqlAlreadyStartsWithExec_DoesNotDuplicateExec()
    {
        var repository = new TestSqlRepository();
        var parameter = new SqlParameter("@Id", SqlDbType.Int) { Value = 5 };

        var sql = repository.ExposedGetFullSql("EXEC dbo.MyProc", CommandType.StoredProcedure, parameter);

        Assert.AreEqual(1, CountOccurrences(sql, "EXEC "));
    }

    [TestMethod]
    public void TextCommand_FormatsStringAndNullParameters()
    {
        var repository = new TestSqlRepository();
        var title = new SqlParameter("@Title", SqlDbType.NVarChar, 0) { Value = "O'Brian" };
        var optional = new SqlParameter("@Optional", SqlDbType.Int) { Value = null };

        var sql = repository.ExposedGetFullSql("SELECT 1", CommandType.Text, title, optional);

        StringAssert.Contains(sql, "DECLARE @Title AS NVarChar(7) = 'O''Brian';");
        StringAssert.Contains(sql, "DECLARE @Optional AS Int = NULL;");
        StringAssert.EndsWith(sql, "SELECT 1");
    }

    [TestMethod]
    public void TextCommand_FormatsDateOnlyParameterAsDateTime()
    {
        var repository = new TestSqlRepository();
        var date = new SqlParameter("@Created", SqlDbType.Date) { Value = new DateOnly(2026, 3, 6) };

        var sql = repository.ExposedGetFullSql("SELECT 1", CommandType.Text, date);

        StringAssert.Contains(sql, "DECLARE @Created AS DateTime = '2026-03-06 00:00:00';");
    }

    [TestMethod]
    public void TextCommand_FormatsStructuredParameterWithInsertStatements()
    {
        var repository = new TestSqlRepository();
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "A");
        table.Rows.Add(2, "B");

        var tvp = new SqlParameter("@Items", SqlDbType.Structured)
        {
            TypeName = "dbo.IntStringList",
            Value = table
        };

        var sql = repository.ExposedGetFullSql("SELECT 1", CommandType.Text, tvp);

        StringAssert.Contains(sql, "-- Table-valued parameter: @Items");
        StringAssert.Contains(sql, "DECLARE @Items AS dbo.IntStringList;");
        StringAssert.Contains(sql, "INSERT INTO @Items ([Id], [Name]) VALUES");
        StringAssert.Contains(sql, "(1, 'A')");
        StringAssert.Contains(sql, "(2, 'B')");
    }

    private static int CountOccurrences(string text, string value)
    {
        var count = 0;
        var startIndex = 0;

        while ((startIndex = text.IndexOf(value, startIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            count++;
            startIndex += value.Length;
        }

        return count;
    }
}
