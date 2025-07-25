using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Configuration;

using System.Threading.Tasks;
using MichaelKappel.Repository.Interfaces;
using Microsoft.Extensions.Options;
using MichaelKappel.Repositories.Common.Models.Configuration;
using MichaelKappel.Repositories.Common.Models;
using MichaelKappel.Repository.Interfaces.Models;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace MichaelKappel.Repository.Bases
{
    #region SqlRepositoryBase
    public abstract class SqlRepositoryBase<T>
    {
        protected readonly string _connectionString;
        protected readonly DatabaseOptionModel? _databaseOptions;

        public SqlRepositoryBase(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public SqlRepositoryBase(IOptions<DatabaseOptionModel> databaseOptions)
        {
            this._databaseOptions = databaseOptions.Value;
            this._connectionString = string.Empty;
        }

        protected SqlConnection GetSqlConnection()
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
            {
                return new SqlConnection(_connectionString);
            }
            else
            {
                return new SqlConnection(_databaseOptions!.DefaultConnection);
            }
        }

        private SqlParameter AdjustParameterType(SqlParameter originalParameter)
        {
            SqlParameter adjustedParameter = new SqlParameter
            {
                ParameterName = originalParameter.ParameterName,
                Direction = originalParameter.Direction,
                Size = originalParameter.Size
            };

            // Handle DateOnly -> DateTime conversion
            if (originalParameter.Value is DateOnly dateOnlyValue)
            {
                adjustedParameter.Value = new DateTime(dateOnlyValue.Year, dateOnlyValue.Month, dateOnlyValue.Day);
                adjustedParameter.SqlDbType = SqlDbType.DateTime;
            }
            // Handle table-valued parameter (TVP)
            else if (originalParameter.SqlDbType == SqlDbType.Structured)
            {
                adjustedParameter.SqlDbType = SqlDbType.Structured;
                adjustedParameter.TypeName = originalParameter.TypeName; // Preserve the type name for TVPs
                adjustedParameter.Value = originalParameter.Value;       // Ensure the DataTable is correctly passed
            }
            else
            {
                // For other data types, just copy the value and the data type
                adjustedParameter.Value = originalParameter.Value;
                adjustedParameter.SqlDbType = originalParameter.SqlDbType;
            }

            return adjustedParameter;
        }


        private String FormatSqlParameter(SqlParameter param)
        {
            switch (param.SqlDbType)
            {
                case SqlDbType.DateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return $"'{((DateTime)param.Value):yyyy-MM-dd HH:mm:ss}'";  // Standard SQL datetime formatting

                case SqlDbType.Char:
                case SqlDbType.VarChar:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.NText:
                    return $"'{param.Value?.ToString()?.Replace("'", "''") ?? ""}'";  // Handle single quotes in strings

                default:
                    return param.Value.ToString();
            }
        }

        private String FormatSqlValue(Object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }

            if (value is DateTime dt)
            {
                return $"'{dt:yyyy-MM-dd HH:mm:ss}'";
            }

            if (value is DateOnly dateOnly)
            {
                return $"'{dateOnly:yyyy-MM-dd}'";
            }

            if (value is String stringValue)
            {
                return $"'{stringValue.Replace("'", "''")}'";
            }

            return value.ToString();
        }

        private String GetSqlType(SqlParameter param)
        {
            // Adding length for variable character types
            if (param.SqlDbType == SqlDbType.NVarChar || param.SqlDbType == SqlDbType.VarChar || param.SqlDbType == SqlDbType.NChar || param.SqlDbType == SqlDbType.Char)
            {
                Int32 size = param.Size > 0 ? param.Size : param.Value?.ToString()?.Length ?? 0;
                // Avoid invalid zero length declarations
                if (size == 0)
                {
                    size = 1;
                }
                return $"{param.SqlDbType}({size})";
            }

            // Special case for Table-Valued Parameters
            if (param.SqlDbType == SqlDbType.Structured)
            {
                return param.TypeName;  // Return the defined type name for TVP
            }

            return param.SqlDbType.ToString();
        }


        protected String GetFullSql(String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return this.GetFullSql(sql, commandType, parameters.ToArray());
        }

        protected String GetFullSql(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            List<SqlParameter> commandParameters = new();
            StringBuilder result = new("\r\n\r\n");

            if (parameters != null && parameters.Any())
            {
                foreach (SqlParameter parameterOriginal in parameters)
                {
                    commandParameters.Add(AdjustParameterType(parameterOriginal));
                }

                foreach (var commandParameter in commandParameters)
                {
                    if (commandParameter.SqlDbType == SqlDbType.Structured)
                    {
                        // Handle table-valued parameters
                        var paramName = commandParameter.ParameterName.Replace("@", "").Replace(" ", String.Empty);
                        result.AppendLine($"-- Table-valued parameter: {commandParameter.ParameterName}");
                        result.AppendLine($"DECLARE @{paramName} AS {commandParameter.TypeName};");

                        if (commandParameter.Value is DataTable dataTable && dataTable.Columns.Count > 0)
                        {
                            var columnNames = String.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => $"[{c.ColumnName}]").ToArray());
                            result.AppendLine($"INSERT INTO @{paramName} ({columnNames}) VALUES");

                            List<string> valueLines = new();
                            foreach (DataRow row in dataTable.Rows)
                            {
                                var values = row.ItemArray.Select(val => FormatSqlValue(val));
                                valueLines.Add($"({String.Join(", ", values)})");
                            }

                            if (valueLines.Any())
                            {
                                result.AppendLine(String.Join(",\n", valueLines) + ";");
                            }
                        }
                    }
                    else
                    {
                        // For regular parameters, handle scalar types
                        String value = commandParameter.Value != null ? FormatSqlParameter(commandParameter) : "NULL";
                        String parameterNamePart = commandParameter.ParameterName.Replace("@", "").Replace(" ", String.Empty);

                        result.Append($"DECLARE @{parameterNamePart} AS {this.GetSqlType(commandParameter)} = {value};\r\n");
                    }
                }
            }

            // Handle stored procedure formatting when explicitly indicated
            String trimmedSql = sql?.Trim() ?? String.Empty;
            Boolean startsWithExec = trimmedSql.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase);

            if (startsWithExec)
            {
                // Remove existing EXEC keyword for consistent output
                trimmedSql = trimmedSql.Substring(4).Trim();
            }

            if (commandType == CommandType.StoredProcedure)
            {
                List<string> execParams = new();

                foreach (var commandParameter in commandParameters)
                {
                    string parameterNamePart = commandParameter.ParameterName.Replace("@", "").Replace(" ", string.Empty);
                    execParams.Add($"@{parameterNamePart} = @{parameterNamePart}");
                }

                result.Append("EXEC ");
                result.Append(trimmedSql);

                if (execParams.Any())
                {
                    result.Append(" ");
                    result.Append(String.Join(", ", execParams));
                }
            }
            else
            {
                result.Append(sql);
            }

            return result.ToString();
        }


        protected virtual T GetFirstModel(String storedProcedure, params SqlParameter[] parameters)
        {
            return GetFirstModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetFirstModel(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = GetModels(storedProcedure, commandType, parameters);
            if (results != default(List<T>))
            {
                result = results[0];
            }
            return result;
        }


        protected virtual async Task<T> GetModelAsync(String storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<T> GetModelAsync(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            T result = default;
            var results = await GetModelsAsync(storedProcedure, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    result = results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception(storedProcedure + " result count greater than 1");
                }
            }
            return result;
        }

        protected virtual async Task<IList<T>> GetModelsAsync(String storedProcedure, params SqlParameter[] parameters)
        {
            return await GetModelsAsync(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual async Task<IList<T>> GetModelsAsync(String storedProcedure, CommandType commandType, params SqlParameter[] parameters)
        {
            var results = default(List<T>);
            // Connection and command creation per thread.
            using (SqlConnection connection = GetSqlConnection())
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = commandType;
                    command.CommandTimeout = 600;

                    if (parameters != null && parameters.Any())
                    {
                        foreach (SqlParameter parameter in parameters)
                        {
                            command.Parameters.Add(AdjustParameterType(parameter));
                        }
                    }

                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        try
                        {
                            if (reader.HasRows)
                            {
                                if (HasError(reader))
                                {
                                    return null;
                                }

                                results = new List<T>();
                                while (await reader.ReadAsync())
                                {
                                    results.Add(CreateInfoFromReader(reader));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("Error reading data", ex);
                        }
                        finally
                        {
                            await reader.CloseAsync();
                        }
                    }
                }
            }
            return (results ?? new List<T>()).AsReadOnly();
        }


        protected virtual T GetModel(String storedProcedure, IList<SqlParameter> parameters)
        {
            return GetModel(storedProcedure, parameters.ToArray());
        }

        protected virtual T GetModel(String storedProcedure, params SqlParameter[] parameters)
        {
            return GetModel(storedProcedure, CommandType.StoredProcedure, parameters);
        }

        protected virtual T GetModel(String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return GetModel(sql, commandType, parameters.ToArray());
        }

        protected virtual T GetModelOrNull(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            IList<T> results = GetModels(sql, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    return (T)results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception($" {sql} result count of {results.Count} expected 1");
                }
            }
            return (T)(Object)null;
        }

        protected virtual T GetModel(String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            IList<T> results = GetModels(sql, commandType, parameters);
            if (results != default(List<T>) && results.Any())
            {
                if (results.Count == 1)
                {
                    return (T)results[0];
                }
                else if (results.Count > 0)
                {
                    throw new Exception($" {sql} result count of {results.Count} expected 1");
                }
            }
            string className = this.GetType().Name;  // Gets the name of the current instance type

#if DEBUG
            throw new Exception($"{className} No result found for {this.GetFullSql(sql, commandType, parameters)}");
#else
            throw new Exception($"No result found on GetModel");
#endif
        }

        protected virtual IList<T> GetModels(String storedProcedure, IList<SqlParameter> parameters)
        {
            return GetModels(storedProcedure, parameters.ToArray());
        }

        protected virtual IList<T> GetModels(String storedProcedure, SqlParameter[] parameters)
        {
            return GetModels(storedProcedure, CommandType.StoredProcedure, parameters);
        }
        protected virtual IList<T> GetModels(String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return GetModels(sql, commandType, parameters.ToArray());
        }

        protected virtual IList<T> GetModels(String sql, CommandType commandType, SqlParameter[] parameters, Int32 retries = 0)
        {
            try
            {
                var results = default(List<T>);
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            //try
                            //{
                            if (reader.HasRows)
                            {
                                if (HasError(reader))
                                {
                                    return null;
                                }

                                results = new List<T>();
                                while (reader.Read())
                                {
                                    results.Add(CreateInfoFromReader(reader));
                                }
                            }
                            //}
                            //catch (Exception ex)
                            //{
                            //    throw ex;
                            //}
                            //finally
                            //{
                            //    reader.Close();
                            //}
                        }
                    }
                }
                return results ?? new List<T>();
            }
            catch (Exception ex)
            {
                if(retries < 4)
                {
                    return this.GetModels(sql, commandType, parameters, retries += 1);
                }
                else
                {
                    throw new Exception(GetFullSql(sql, commandType, parameters), ex);
                }
            }
        }
        protected virtual int ExecuteNonQuery(String sql, CommandType commandType,  SqlParameter[] parameters, Int32 retries = 0)
        {
            try
            {

                int result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        result = command.ExecuteNonQuery();
                    }
                }
                return result;
            }
            catch
            {
                if (retries < 4)
                {
                    return this.ExecuteNonQuery(sql, commandType, parameters, retries += 1);
                }
                else
                {
#if DEBUG
                    throw new Exception(this.GetFullSql(sql, commandType, parameters));
#else
                            throw;
#endif
                }
            }

        }

        protected virtual async Task<int> ExecuteNonQueryAsync(String sql, CommandType commandType, SqlParameter[] parameters, Int32 retries = 0)
        {
            try
            {
                int result;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        await connection.OpenAsync();
                        result = await command.ExecuteNonQueryAsync();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                {
                    if (retries < 4)
                    {
                        return await this.ExecuteNonQueryAsync(sql, commandType, parameters, retries += 1);
                    }
                    else
                    {
#if DEBUG
                        throw new Exception(this.GetFullSql(sql, commandType, parameters), ex);
#else
            throw;
#endif
                    }
                }
            }
        }

        protected virtual Ts ExecuteScalar<Ts>(String sql, CommandType commandType, SqlParameter[] parameters, Int32 retries = 0)
        {
            try
            {

                Ts result = default;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        connection.Open();
                        result = (Ts)command.ExecuteScalar();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                {
                    if (retries < 4)
                    {
                        return ExecuteScalar<Ts>(sql, commandType, parameters, retries += 1);
                    }
                    else
                    {
#if DEBUG
                        throw new Exception(this.GetFullSql(sql, commandType, parameters), ex);
#else
            throw;
#endif
                    }
                }
            }
        }
        protected virtual async Task<Ts> ExecuteScalarAsync<Ts>(String sql, CommandType commandType, SqlParameter[] parameters)
        {
            try
            {
                Ts result;
                using (SqlConnection connection = GetSqlConnection())
                {
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.CommandType = commandType;
                        command.CommandTimeout = 600;

                        if (parameters != null && parameters.Any())
                        {
                            foreach (SqlParameter parameter in parameters)
                            {
                                command.Parameters.Add(AdjustParameterType(parameter));
                            }
                        }

                        await connection.OpenAsync();
                        object resultRaw = await command.ExecuteScalarAsync();
                        result = (Ts)resultRaw!;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
#if DEBUG
                throw new Exception(this.GetFullSql(sql, commandType, parameters), ex);
#else
            throw;
#endif
            }
        }

        protected virtual int Execute(String storedProcedure, params SqlParameter[] parameters)
        {
            return ExecuteNonQuery(storedProcedure, CommandType.StoredProcedure, parameters);
        }
        protected virtual DT ReadAs<DT>(IDataReader reader, String fieldName)
        {
            Type expectedType = typeof(DT);
            if(reader.GetOrdinal(fieldName) < 0)
            {
                throw new Exception($"FiiedName {fieldName} Not found");
            }
            var rawFieldValue = reader[fieldName];
            try
            {
                if (rawFieldValue == DBNull.Value)
                {
                    return default;
                }
                if (expectedType == typeof(bool) && rawFieldValue.GetType() == typeof(byte))
                {
                    return (DT)(object)((byte)rawFieldValue == 1 ? true : false);
                }
                if (expectedType == typeof(DateTime) && rawFieldValue.GetType() == typeof(String))
                {
                    DateTime dt;
                    if (DateTime.TryParse(rawFieldValue.ToString(), out dt))
                    {
                        return (DT)(object)dt;
                    }
                    else
                    {
                        return (DT)(object)default(DateTime);
                    }
                }
                else if (expectedType == typeof(DateOnly))
                {
                    if (rawFieldValue.GetType() == typeof(string))
                    {
                        if (DateOnly.TryParse(rawFieldValue.ToString(), out DateOnly dol))
                        {
                            return (DT)(object)dol;
                        }
                        else
                        {
                            if (DateTime.TryParse(rawFieldValue.ToString(), out DateTime dt))
                            {
                                return (DT)(object)DateOnly.FromDateTime(dt);
                            }
                            else
                            {
                                return (DT)(object)default(DateOnly);
                            }
                        }
                    }
                    else
                    {
                        return (DT)(object)DateOnly.FromDateTime((DateTime)rawFieldValue);
                    }
                }
                else if (expectedType == typeof(bool) && rawFieldValue.GetType() == typeof(String))
                {
                    return (DT)(object)(rawFieldValue.ToString().ToLower() == "true" || rawFieldValue.ToString().ToLower() == "y" ? true : false);
                }
                else if (expectedType == typeof(bool) && (rawFieldValue.GetType() == typeof(int) || rawFieldValue.GetType() == typeof(short) || rawFieldValue.GetType() == typeof(byte)))
                {
                    return (DT)(object)(rawFieldValue.ToString() == "0" ? false : true);
                }
                else if (expectedType == typeof(int) && rawFieldValue.GetType() == typeof(byte))
                {
                    return (DT)(object)int.Parse(rawFieldValue.ToString()!);
                }
                else if (expectedType == typeof(int) && rawFieldValue.GetType() == typeof(short))
                {
                    return (DT)(object)int.Parse(rawFieldValue.ToString()!);
                }
                else if (expectedType == typeof(int))
                {
                    return (DT)(object)int.Parse((rawFieldValue.ToString() ?? String.Empty).Trim());
                }
                else if ((expectedType == typeof(decimal?) || expectedType == typeof(decimal)) && rawFieldValue.GetType() == typeof(double))
                {
                    String valueAsString = rawFieldValue.ToString()!;
                    if (valueAsString.Contains("E-"))
                    {
                        return (DT)(object)0M;
                    }
                    return (DT)(object)Math.Round(decimal.Parse(valueAsString), 6, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return (DT)rawFieldValue;
                }
            }
            catch (Exception e)
            {
                string className = this.GetType().Name;  // Gets the name of the current instance type
                throw new Exception($"Error in {className}: fieldName {fieldName}, value {rawFieldValue}, Databse type {rawFieldValue.GetType()} C# type {typeof(DT)}", e);
            }
        }
        protected bool HasError(IDataReader reader)
        {
            if (ReaderContainsColumn(reader, "ErrorNumber"))
            {
                while (reader.Read())
                {
                    int errorNumber = ReadAs<int>(reader, "ErrorNumber");
                    int errorSeverity = ReadAs<int>(reader, "ErrorSeverity");
                    int errorState = ReadAs<int>(reader, "ErrorState");
                    String errorProcedure = ReadAs<String>(reader, "ErrorProcedure");
                    int errorLine = ReadAs<int>(reader, "ErrorLine");
                    String errorMessage = ReadAs<String>(reader, "ErrorMessage");

                    HandleSqlError(errorNumber, errorSeverity, errorState, errorProcedure, errorLine, errorMessage);
                }
                return true;
            }
            return false;
        }
        protected virtual void HandleSqlError(int ErrorNumber, int ErrorSeverity, int ErrorState, String ErrorProcedure, int ErrorLine, String ErrorMessage)
        {
            throw new Exception(ErrorMessage);
        }
        public static bool ReaderContainsColumn(IDataReader reader, String name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(name, StringComparison.CurrentCultureIgnoreCase)) return true;
            }
            return false;
        }

        protected abstract T CreateInfoFromReader(SqlDataReader reader);
    }
    #endregion SqlRepositoryBase

    #region SqlRepositoryPagingBase
    public abstract class SqlRepositoryPagingBase<T> : SqlRepositoryBase<T> where T : class
    {
        private ObjectCache cache = MemoryCache.Default;

        protected internal string SqlPaging = $" OFFSET @PageIndex ROWS FETCH NEXT @PageSize ROWS ONLY";

        protected SqlRepositoryPagingBase(string connectionString) : base(connectionString)
        {
        }

        protected SqlRepositoryPagingBase(IOptions<DatabaseOptionModel> databaseOptions) : base(databaseOptions)
        {
        }

        private (String Sql, SqlParameter[] Parameters) GetRecordCountQuery(String sql, SqlParameter[] parameters)
        {

            List<SqlParameter> countParameters = new();
            foreach (SqlParameter p in parameters)
            {
                countParameters.Add(new SqlParameter(p.ParameterName, p.Value));
            }

            String innerQuery = sql.Replace("  ", " ");

            Match match = Regex.Match(innerQuery, @"(?i)\bSELECT\b(?>[^()]|\((?<Depth>)|\)(?<-Depth>))*(?(Depth)(?!))\bFROM\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                innerQuery = innerQuery.Remove(match.Index, match.Length)
                                         .Insert(match.Index, "SELECT 1 AS FakeColumn FROM");
            }

            String sqlOrderBy = "ORDER BY";
            if (innerQuery.Contains(sqlOrderBy, StringComparison.OrdinalIgnoreCase))
            {
                String[] sqlSplit = Regex.Split(innerQuery, sqlOrderBy, RegexOptions.IgnoreCase);

                innerQuery = sqlSplit[0];
            }

            String countSql = @$"SELECT COUNT(cteRecordCount.FakeColumn) FROM ({innerQuery}) AS cteRecordCount";

            return (countSql, countParameters.ToArray());
        }


        private int GetRecordCount(String sql, SqlParameter[] parameters, bool isCountCached = true)
        {
            if (isCountCached)
            {
                String cacheKey = GenerateCacheKey(sql, parameters);

                if (cache.Contains(cacheKey))
                {
                    return (int)cache.Get(cacheKey);
                }
                else
                {
                    int recordCount = ExecuteCountQuery(sql, parameters);
                    CacheItemPolicy cachePolicy = new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    };
                    cache.Add(cacheKey, recordCount, cachePolicy);
                    return recordCount;
                }
            }
            else
            {
                return ExecuteCountQuery(sql, parameters);
            }
        }

        private int ExecuteCountQuery(String sql, SqlParameter[] parameters)
        {
            (String Sql, SqlParameter[] Parameters) countingQuery = GetRecordCountQuery(sql, parameters);
            return ExecuteScalar<int>(countingQuery.Sql, CommandType.Text, countingQuery.Parameters);
        }

        private async Task<int> ExecuteCountQueryAsync(String sql, SqlParameter[] parameters)
        {
            (String Sql, SqlParameter[] Parameters) countingQuery = GetRecordCountQuery(sql, parameters);

            return await ExecuteScalarAsync<int>(countingQuery.Sql, CommandType.Text, countingQuery.Parameters);
        }

        private String GenerateCacheKey(String sql, SqlParameter[] parameters)
        {
            String parameterKey = String.Join("_", parameters.Select(p => p.ParameterName + "=" + p.Value));
            return sql + "_" + parameterKey;
        }

        private async Task<int> GetRecordCountAsync(String sql, SqlParameter[] parameters, bool isCountCached = true)
        {
            if (isCountCached)
            {
                String cacheKey = GenerateCacheKey(sql, parameters);

                if (cache.Contains(cacheKey))
                {
                    return (int)cache.Get(cacheKey);
                }
                else
                {
                    int recordCount = await ExecuteCountQueryAsync(sql, parameters);
                    CacheItemPolicy cachePolicy = new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromHours(1)
                    };
                    cache.Add(cacheKey, recordCount, cachePolicy);
                    return recordCount;
                }
            }
            else
            {
                return await ExecuteCountQueryAsync(sql, parameters);
            }
        }

        protected void AddPaging(IPaging pageing, ref CommandType commandType, ref String sql, ref SqlParameter[] parameters)
        {
            if (commandType != CommandType.Text)
            {
                throw new Exception("SqlRepositoryPagingBase currently only supports commandType of Text");
            }

            if (!sql.Contains(SqlPaging))
            {
                sql += SqlPaging;
            }

            List<SqlParameter> parametersWithPaging = parameters.ToList();

            parametersWithPaging.Add(new SqlParameter("@PageIndex", pageing.PageIndex));
            parametersWithPaging.Add(new SqlParameter("@PageSize", pageing.PageSize));

            parameters = parametersWithPaging.ToArray();
        }

        protected virtual PagingResultsModel<T> GetPagingResults(IPaging pageing, String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return this.GetPagingResults(pageing, sql, commandType, parameters.ToArray());
        }

        protected virtual PagingResultsModel<T> GetPagingResults(IPaging pageing, String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            int totalRecordCount = GetRecordCount(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            IList<T> rawResults = GetModels(sql, commandType, parameters);

            return new PagingResultsModel<T>(pageing, totalRecordCount, rawResults);
        }
        protected virtual async Task<PagingResultsModel<T>> GetPagingResultsAsync(IPaging pageing, String sql, CommandType commandType, IList<SqlParameter> parameters)
        {
            return await this.GetPagingResultsAsync(pageing, sql, commandType, parameters.ToArray());
        }

        protected virtual async Task<PagingResultsModel<T>> GetPagingResultsAsync(IPaging pageing, String sql, CommandType commandType, params SqlParameter[] parameters)
        {
            Task<int> totalRecordCount = GetRecordCountAsync(sql, parameters);

            AddPaging(pageing, ref commandType, ref sql, ref parameters);

            Task<IList<T>> rawResults = GetModelsAsync(sql, commandType, parameters);

            await Task.WhenAll(totalRecordCount, rawResults);

            return new PagingResultsModel<T>(pageing, await totalRecordCount, await rawResults);
        }
    }
    #endregion SqlRepositoryPagingBase
}
