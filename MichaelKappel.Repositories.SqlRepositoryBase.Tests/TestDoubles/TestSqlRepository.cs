using System.Data;
using Microsoft.Data.SqlClient;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repositories.SqlRepositoryBase.Models;
using MichaelKappel.Repository.Bases;
using MichaelKappel.Repository.Interfaces.Models;

namespace MichaelKappel.Repositories.SqlRepositoryBase.Tests.TestDoubles;

internal sealed class TestSqlRepository : SqlRepositoryBase<TestSqlRepository.TestRow>
{
    public sealed class TestRow
    {
        public int Id { get; init; }
    }

    public IList<TestRow> ModelsToReturn { get; set; } = new List<TestRow>();
    public (int Number, int Severity, int State, string Procedure, int Line, string Message)? LastSqlError { get; private set; }

    public TestSqlRepository()
    : base("Server=(local);Database=Test;Trusted_Connection=True;")
    {
    }

    public string ExposedGetFullSql(string sql, CommandType commandType, params SqlParameter[] parameters)
    {
        return GetFullSql(sql, commandType, parameters);
    }

    public TValue ExposedReadAs<TValue>(IDataReader reader, string fieldName)
    {
        return ReadAs<TValue>(reader, fieldName);
    }

    public bool ExposedHasError(IDataReader reader)
    {
        return HasError(reader);
    }

    public TestRow? ExposedGetModelOrNull(string sql, CommandType commandType, params SqlParameter[] parameters)
    {
        return GetModelOrNull(sql, commandType, parameters);
    }

    public TestRow ExposedGetModel(string sql, CommandType commandType, params SqlParameter[] parameters)
    {
        return GetModel(sql, commandType, parameters);
    }

    protected override IList<TestRow> GetModels(string sql, CommandType commandType, SqlParameter[] parameters, int retries = 0)
    {
        return ModelsToReturn;
    }

    protected override void HandleSqlError(int errorNumber, int errorSeverity, int errorState, string errorProcedure, int errorLine, string errorMessage)
    {
        LastSqlError = (errorNumber, errorSeverity, errorState, errorProcedure, errorLine, errorMessage);
    }

    protected override TestRow CreateInfoFromReader(SqlDataReader reader)
    {
        return new TestRow();
    }
}

internal sealed class TestPagingRepository : SqlRepositoryPagingBase<TestSqlRepository.TestRow>
{
    public int CountToReturn { get; set; }
    public int ExecuteScalarCallCount { get; private set; }
    public string? LastCountSql { get; private set; }
    public SqlParameter[] LastCountParameters { get; private set; } = Array.Empty<SqlParameter>();

    public string? LastModelsSql { get; private set; }
    public CommandType LastModelsCommandType { get; private set; }
    public SqlParameter[] LastModelsParameters { get; private set; } = Array.Empty<SqlParameter>();

    public IList<TestSqlRepository.TestRow> ModelsToReturn { get; set; } = new List<TestSqlRepository.TestRow>();

    public TestPagingRepository()
        : base("Server=(local);Database=Test;Trusted_Connection=True;")
    {
    }

    public void ExposedAddPaging(IPaging paging, ref CommandType commandType, ref string sql, ref SqlParameter[] parameters)
    {
        AddPaging(paging, ref commandType, ref sql, ref parameters);
    }

    public PagingResultsModel<TestSqlRepository.TestRow> ExposedGetPagingResults(IPaging paging, string sql, CommandType commandType, params SqlParameter[] parameters)
    {
        return GetPagingResults(paging, sql, commandType, parameters);
    }

    public Task<PagingResultsModel<TestSqlRepository.TestRow>> ExposedGetPagingResultsAsync(IPaging paging, string sql, CommandType commandType, params SqlParameter[] parameters)
    {
        return GetPagingResultsAsync(paging, sql, commandType, parameters);
    }

    protected override IList<TestSqlRepository.TestRow> GetModels(string sql, CommandType commandType, SqlParameter[] parameters, int retries = 0)
    {
        LastModelsSql = sql;
        LastModelsCommandType = commandType;
        LastModelsParameters = parameters;
        return ModelsToReturn;
    }

    protected override Task<IList<TestSqlRepository.TestRow>> GetModelsAsync(string storedProcedure, CommandType commandType, params SqlParameter[] parameters)
    {
        LastModelsSql = storedProcedure;
        LastModelsCommandType = commandType;
        LastModelsParameters = parameters;
        return Task.FromResult(ModelsToReturn);
    }

    protected override Ts ExecuteScalar<Ts>(string sql, CommandType commandType, SqlParameter[] parameters, int retries = 0)
    {
        ExecuteScalarCallCount++;
        LastCountSql = sql;
        LastCountParameters = parameters;
        return (Ts)Convert.ChangeType(CountToReturn, typeof(Ts));
    }

    protected override Task<Ts> ExecuteScalarAsync<Ts>(string sql, CommandType commandType, SqlParameter[] parameters)
    {
        ExecuteScalarCallCount++;
        LastCountSql = sql;
        LastCountParameters = parameters;
        var result = (Ts)Convert.ChangeType(CountToReturn, typeof(Ts));
        return Task.FromResult(result);
    }

    protected override TestSqlRepository.TestRow CreateInfoFromReader(SqlDataReader reader)
    {
        return new TestSqlRepository.TestRow();
    }
}
