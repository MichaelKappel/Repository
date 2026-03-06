using System.Data;
using MichaelKappel.Repositories.SqlRepositoryBase.Tests.TestDoubles;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests;

[TestClass]
public class SqlRepositoryBaseTests
{
    [TestMethod]
    public void ReaderContainsColumn_IsCaseInsensitive()
    {
        using var reader = CreateReader(new Dictionary<string, object?>
        {
            ["ErrorNumber"] = 1
        });

        Assert.IsTrue(MichaelKappel.Repository.Bases.SqlRepositoryBase<TestSqlRepository.TestRow>.ReaderContainsColumn(reader, "errornumber"));
        Assert.IsFalse(MichaelKappel.Repository.Bases.SqlRepositoryBase<TestSqlRepository.TestRow>.ReaderContainsColumn(reader, "DoesNotExist"));
    }

    [TestMethod]
    public void ReadAs_ConvertsSupportedTypes()
    {
        var repository = new TestSqlRepository();
        using var reader = CreateReader(new Dictionary<string, object?>
        {
            ["BoolFromByte"] = (byte)1,
            ["BoolFromString"] = "Y",
            ["DateOnlyFromString"] = "2026-03-06",
            ["DateOnlyFromDateTime"] = new DateTime(2026, 3, 6),
            ["IntFromShort"] = (short)12,
            ["RoundedDecimal"] = 3.1415926535,
            ["ScientificDecimal"] = 1.25E-08,
            ["NullValue"] = DBNull.Value
        });

        Assert.IsTrue(repository.ExposedReadAs<bool>(reader, "BoolFromByte"));
        Assert.IsTrue(repository.ExposedReadAs<bool>(reader, "BoolFromString"));
        Assert.AreEqual(new DateOnly(2026, 3, 6), repository.ExposedReadAs<DateOnly>(reader, "DateOnlyFromString"));
        Assert.AreEqual(new DateOnly(2026, 3, 6), repository.ExposedReadAs<DateOnly>(reader, "DateOnlyFromDateTime"));
        Assert.AreEqual(12, repository.ExposedReadAs<int>(reader, "IntFromShort"));
        Assert.AreEqual(3.141593M, repository.ExposedReadAs<decimal>(reader, "RoundedDecimal"));
        Assert.AreEqual(0M, repository.ExposedReadAs<decimal>(reader, "ScientificDecimal"));
        Assert.IsNull(repository.ExposedReadAs<string>(reader, "NullValue"));
    }

    [TestMethod]
    public void ReadAs_MissingField_ThrowsArgumentException()
    {
        var repository = new TestSqlRepository();
        using var reader = CreateReader(new Dictionary<string, object?>
        {
            ["Present"] = 1
        });

        var ex = Assert.ThrowsException<ArgumentException>(() => repository.ExposedReadAs<int>(reader, "Missing"));

        StringAssert.Contains(ex.Message, "Missing");
    }

    [TestMethod]
    public void HasError_WhenErrorColumnsExist_InvokesHandleSqlError()
    {
        var repository = new TestSqlRepository();
        using var reader = CreateReader(new Dictionary<string, object?>
        {
            ["ErrorNumber"] = 500,
            ["ErrorSeverity"] = 16,
            ["ErrorState"] = 2,
            ["ErrorProcedure"] = "dbo.SaveItem",
            ["ErrorLine"] = 44,
            ["ErrorMessage"] = "boom"
        }, moveToFirstRecord: false);

        var hasError = repository.ExposedHasError(reader);

        Assert.IsTrue(hasError);
        Assert.IsNotNull(repository.LastSqlError);
        Assert.AreEqual(500, repository.LastSqlError.Value.Number);
        Assert.AreEqual("boom", repository.LastSqlError.Value.Message);
    }

    [TestMethod]
    public void HasError_WhenColumnsAreMissing_ReturnsFalse()
    {
        var repository = new TestSqlRepository();
        using var reader = CreateReader(new Dictionary<string, object?>
        {
            ["Id"] = 1
        }, moveToFirstRecord: false);

        var hasError = repository.ExposedHasError(reader);

        Assert.IsFalse(hasError);
    }

    [TestMethod]
    public void GetModelOrNull_ReturnsNull_WhenNoRows()
    {
        var repository = new TestSqlRepository
        {
            ModelsToReturn = new List<TestSqlRepository.TestRow>()
        };

        var model = repository.ExposedGetModelOrNull("SELECT 1", CommandType.Text);

        Assert.IsNull(model);
    }

    [TestMethod]
    public void GetModelOrNull_ReturnsSingleRow_WhenOneRowExists()
    {
        var repository = new TestSqlRepository
        {
            ModelsToReturn = new List<TestSqlRepository.TestRow>
            {
                new() { Id = 7 }
            }
        };

        var model = repository.ExposedGetModelOrNull("SELECT 1", CommandType.Text);

        Assert.IsNotNull(model);
        Assert.AreEqual(7, model.Id);
    }

    [TestMethod]
    public void GetModelOrNull_Throws_WhenMultipleRowsExist()
    {
        var repository = new TestSqlRepository
        {
            ModelsToReturn = new List<TestSqlRepository.TestRow>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }
        };

        var ex = Assert.ThrowsException<Exception>(() => repository.ExposedGetModelOrNull("SELECT 1", CommandType.Text));

        StringAssert.Contains(ex.Message, "result count of 2 expected 1");
    }

    [TestMethod]
    public void GetModel_Throws_WhenNoRowsExist()
    {
        var repository = new TestSqlRepository
        {
            ModelsToReturn = new List<TestSqlRepository.TestRow>()
        };

        var ex = Assert.ThrowsException<Exception>(() => repository.ExposedGetModel("SELECT 1", CommandType.Text));

        StringAssert.Contains(ex.Message, "No result found");
    }

    private static IDataReader CreateReader(IDictionary<string, object?> values, bool moveToFirstRecord = true)
    {
        var table = new DataTable();
        foreach (var key in values.Keys)
        {
            table.Columns.Add(key, typeof(object));
        }

        var row = table.NewRow();
        foreach (var pair in values)
        {
            row[pair.Key] = pair.Value ?? DBNull.Value;
        }

        table.Rows.Add(row);

        var reader = table.CreateDataReader();
        if (moveToFirstRecord)
        {
            reader.Read();
        }

        return reader;
    }
}
